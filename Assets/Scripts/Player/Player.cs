using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    
    public int velocity, controller;

    float elapsedTime=0;

    [SerializeField]
    GameObject basicMissile;

    Rigidbody2D rigidbody2d;

    [SerializeField]
    Slider reloadMissileBar;

    public int health;

    [SerializeField]
    Image[] hearts;

    [SerializeField]
    Sprite fullHeart, emptyHeart;

    

    void Start()
    {
        this.rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HealthCheck();
        Movement();
        Shoot();
    }

    //Movimentaçao base do jogador, controlada pelas teclas direcionais.
    void Movement()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.rigidbody2d.velocity = new Vector2(-velocity, this.rigidbody2d.velocity.y);
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.rigidbody2d.velocity = new Vector2(velocity, this.rigidbody2d.velocity.y);
        }
      
        if (controller == 1)
        {
            this.rigidbody2d.velocity = new Vector2(velocity,this.rigidbody2d.velocity.y);
        }

        else if (controller == 2)
        {
            this.rigidbody2d.velocity = new Vector2(-velocity, this.rigidbody2d.velocity.y);
        }

        else
        {
            this.rigidbody2d.velocity = new Vector2(0, this.rigidbody2d.velocity.y);
        }

    }

    //Metodos para o botao de andar (mobile)
    //O controller diz se a nave deve ir pra esquerda ou direita ou ficar parado.
    public void GoLeftDown()
    {
        controller = 2;
    }

    public void GoLeftUp()
    {
        controller = 3;
    }

    public void GoRightDown()
    {
        controller = 1;
    }

    public void GoRightUp()
    {
        controller = 3;
    }

    //Checa a vida do jogador, alem disso organiza os sprites pela quantidade de vida.
    //Na array hearts, o .Lenght é a quantidade de vida maxima do jogador.
    void HealthCheck()
    {
        if (health <= 0)
        {           
            ScreenManager.instance.LoseScreen();
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    //Instancia um missel e limita a frequencia de tiros em que o jogador pode dar.
    public void Shoot()
    {
        if(reloadMissileBar.value == 100)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 missileSpawnPosition = this.transform.position;
                missileSpawnPosition.y += 0.7f;
                GameObject basicMissile = GameObject.Instantiate(this.basicMissile, missileSpawnPosition, Quaternion.identity);
                AudioManager.instance.PlaySound("PlayerShoot");
                reloadMissileBar.value = 0;
            }
        }

        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 0.1f)
            {
                reloadMissileBar.value += 10;
                elapsedTime = 0;
            }
        }

    }

    //Metodo para atirar pelo botao.
    public void ShootButton()
    {
        if(reloadMissileBar.value == 100)
        {
            AudioManager.instance.PlaySound("PlayerShoot");
            Vector3 missileSpawnPosition = this.transform.position;
            missileSpawnPosition.y += 0.7f;
            GameObject basicMissile = GameObject.Instantiate(this.basicMissile, missileSpawnPosition, Quaternion.identity);        
            reloadMissileBar.value = 0;
        }
       
    }

    //Metodo que executa o dano ao jogador se ele colidir com um EnemyMissile e checa se o power up do escudo esta ativo.
    //Caso o power up esteja ativo, ele é destruido e o Player nao leva dano.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy Missile"))
        {
            AudioManager.instance.PlaySound("PlayerDamage");
            GameObject shield = GameObject.FindGameObjectWithTag("ShieldPlayer");
            if (shield != null)
            {
                Destroy(collision.gameObject);
                Destroy(shield);
            }
            else
            {
                health -= 1;
                Destroy(collision.gameObject);
            }
            
        }
    }


    //Trigger que controla quando o Player entra no campo de anti gravidade do boss.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GravityRay"))
        {
            this.rigidbody2d.velocity = new Vector2(this.rigidbody2d.velocity.x, 1);
        }
    }
}

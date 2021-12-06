using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public GameObject winMusic;
    public GameObject loseMusic;
    public GameObject backgroundMusic;

    public ParticleSystem healthIncrease;
    public ParticleSystem healthDecrease;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip chat;

    public AudioClip cogPickupSound;

    public int cogs;

    static int currentScore;

    static int killCount;
    static int level=1;
   
    public int score { get { return currentScore; } }
    public int killNumber{get{return killCount;}}
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public Text scoreText;
    public Text murderText;
    public Text gameOver;
    public Text cogsText;
    public bool winOrlose;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        winMusic.gameObject.SetActive(false);
        loseMusic.gameObject.SetActive(false);
        backgroundMusic.gameObject.SetActive(true);

        currentHealth = maxHealth;
        cogs = 4;
        scoreText.text = "Fixed Robots: "+currentScore.ToString();
        cogsText.text = "Cogs: " + cogs.ToString();
        gameOver.text = "";
        winOrlose = false;
        if(level==3)
        murderText.gameObject.SetActive(true);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {

            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;

            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogs > 0)
            {
                Launch();
                cogs--;
                cogsText.text = "Cogs: " + cogs.ToString();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if(level<3)
                    {
                       character.DisplayDialog();
                        PlaySound(chat); 
                    }
                    
                    if (winOrlose == true)
                    {
                        level++;
                        SceneManager.LoadScene("Main 1");

                    }
                }
            }
            RaycastHit2D hit2 = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("Doggo"));
            if (hit2.collider != null)
            {
                
                Debug.Log("Raycast has hit the object " + hit2.collider.gameObject);
                DoggoController doggo = hit2.collider.GetComponent<DoggoController>();
                if (doggo != null)
                {
                    Debug.Log("Raycast has hit the object " + hit2.collider.gameObject);
                    if(level==3)
                    {
                        doggo.DisplayDialog();
                        PlaySound(chat);
                    }
                    if (winOrlose == true)
                    {
                        level++;
                        SceneManager.LoadScene("Main 2");

                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.R))

        {

            if (winOrlose == true)

            {
                if (currentHealth <= 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
                    if(level==1)
                    {
                        currentScore=0;
                    }
                    if(level==2)
                    {
                        currentScore=4;
                    }
                    if(level==3)
                    {
                        currentScore=8;
                        killCount=0;
                    }
                }
                if (killCount == 4)
                {
                    currentScore=0;
                    killCount=0;
                    level=1;
                    SceneManager.LoadScene("Main");
                }

            }

        }


    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            ParticleSystem damageEffect = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            if (currentHealth < maxHealth)
            {
                ParticleSystem recovery = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            }
        }


        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            speed = 0;
            gameOver.text = " You Lose! Press R to restart";
            backgroundMusic.gameObject.SetActive(false);
            audioSource.mute = audioSource.mute;
            loseMusic.gameObject.SetActive(true);
            winOrlose = true;
            isInvincible = true;

        }
    }

    public void ChangeScore(int scoreAmount)
    {

        currentScore += scoreAmount;
        scoreText.text = "Fixed Robots: " + currentScore.ToString();
        if (currentScore == 4)
        {
            gameOver.text = "Talk to Jambi to visit stage two!";
            winOrlose = true;
            backgroundMusic.gameObject.SetActive(false);
            audioSource.mute = audioSource.mute;
            winMusic.gameObject.SetActive(true);

        }
        if (currentScore == 8)
        {
            gameOver.text = "Talk to Doggo to visit a Bonus Stage";
            winOrlose = true;
            backgroundMusic.gameObject.SetActive(false);
            audioSource.mute = audioSource.mute;
            winMusic.gameObject.SetActive(true);


        }
        if (killCount == 4)
        {
            gameOver.text = "You Win, Game made by Daniel Arias. Press R to Restart";
            winOrlose = true;
            backgroundMusic.gameObject.SetActive(false);
            audioSource.mute = audioSource.mute;
            winMusic.gameObject.SetActive(true);


        }
    }

    public void Kills(int killAmount)
    {

        killCount += killAmount;
        murderText.text = "Goblins Defeated: " + killCount.ToString();
       
        if (killCount == 4)
        {
            gameOver.text = "You Win, Game made by Daniel Arias Press R to Restart";
            winOrlose = true;
            backgroundMusic.gameObject.SetActive(false);
            audioSource.mute = audioSource.mute;
            winMusic.gameObject.SetActive(true);


        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.collider.tag == "collectable")
        {
            cogs += 3;
            cogsText.text = "Cogs: " + cogs.ToString();
            Destroy(collision2D.collider.gameObject);
            PlaySound(cogPickupSound);
        }
    }
}
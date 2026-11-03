using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

[RequireComponent(typeof(Movement), typeof(PlayerInput), typeof(Status))]
public class PlayerController : MonoBehaviour, InterfaceFuncoes
{
    [SerializeField] private Movement movementController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Status playerStatus;
    [SerializeField] private AudioClip beingHitSound;
    [SerializeField] private InterfaceController interfaceController;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Image sliderBackGround;
    [SerializeField] private Color maxHealthColor, minHealthColor;
    
    private Vector3 _direction;
    private Vector3 _rotation;
    private Vector3 _faceDirection;
    private Transform _healthCanvas;
    private static readonly Vector3 HealthBarOffset = new Vector3(0f, 0.9f, -1f);
    private InputAction _moveAction;
    private InputAction _rotateAction;

    private void Start()
    {
        _healthCanvas = playerHealthSlider.GetComponentInParent<Canvas>().transform;
        playerHealthSlider.maxValue = playerStatus.initialHealth;
        HealthBar();

        // No Menu o personagem só aparece na seleção, sem barra de vida.
        if (interfaceController == null) playerHealthSlider.GetComponentInParent<Canvas>().gameObject.SetActive(false);

        _moveAction = playerInput.actions["Move"];
        _rotateAction = playerInput.actions["Look"];
    }

    void LateUpdate()
    {
        // A barra de vida é filha do jogador, que gira com a mira. Mantém ela sempre embaixo na tela.
        if (_healthCanvas != null) _healthCanvas.position = transform.position + HealthBarOffset;
    }

    void Update()
    {
        if (movementController.isMoving) animationController.Move(_direction.magnitude);

        ReceiveInputs();
    }

     void FixedUpdate()
    {
        movementController.Move(_direction, playerStatus.speed);
        movementController.Rotate(_rotation);
        if (_rotation == Vector3.zero) movementController.Face(_faceDirection);
    }

     /// <summary>
     /// Call this function to update players health bar.
     /// </summary>
     private void HealthBar()
     {
         playerHealthSlider.value = playerStatus.health;

         float healthPercent = (float) playerStatus.health / playerStatus.initialHealth;
         Color healthColor = Color.Lerp(minHealthColor, maxHealthColor, healthPercent);

         healthColor.a = 1f;
         sliderBackGround.color = healthColor;
         playerHealthSlider.fillRect.GetComponent<Image>().color = healthColor;
     }

     /// <summary>
     /// Call this function to update input's values to player movement and rotation.
     /// </summary>
     private void ReceiveInputs()
     {
         // O personagem da tela de seleção (Menu) não tem InterfaceController e não deve andar nem atirar.
         if (interfaceController == null)
         {
             _direction = Vector3.zero;
             _rotation = Vector3.zero;
             return;
         }

         Vector2 moveInput = _moveAction.ReadValue<Vector2>();
         Vector2 rotateInput = _rotateAction.ReadValue<Vector2>();
         _direction = new Vector3(moveInput.x, 0, moveInput.y);

         // Teclado e mouse: o personagem sempre olha para o mouse e só atira com o botão esquerdo pressionado.
         Vector2 mouseDirection = MouseDirection();
         _faceDirection = new Vector3(mouseDirection.x, 0, mouseDirection.y);

         var mouse = Mouse.current;
         bool isShooting = mouse != null && mouse.leftButton.isPressed && !IsPointerOverUI();
         if (rotateInput == Vector2.zero && isShooting) rotateInput = mouseDirection;

         _rotation = new Vector3(rotateInput.x, 0, rotateInput.y);
     }

     /// <summary>
     /// Call this function to get the direction (on the ground plane) from the player to the mouse.
     /// </summary>
     private Vector2 MouseDirection()
     {
         var mouse = Mouse.current;
         if (mouse == null || Camera.main == null) return Vector2.zero;

         Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
         var groundPlane = new Plane(Vector3.up, transform.position);
         if (!groundPlane.Raycast(ray, out float distance)) return Vector2.zero;

         Vector3 toMouse = ray.GetPoint(distance) - transform.position;
         return new Vector2(toMouse.x, toMouse.z).normalized;
     }

     private static bool IsPointerOverUI()
     {
         return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
     }

     /// <summary>
     /// Call this function to receive damage.
     /// </summary>
     public void TakeDamage(int damage)
    {
        playerStatus.health -= damage;
        ControlaAudio.instancia.PlayOneShot(beingHitSound);
        HealthBar();

        if (playerStatus.health <= 0)
        {
            //Die();
        }

    }
    
    //Maybe change to event?
    public void Die()
    {
        interfaceController.GameOver();
    }

    /// <summary>
    /// Call this function to restore players health.
    /// </summary>
    public void Heal(int healingQuantity)
    {
        playerStatus.health += healingQuantity;
        if (playerStatus.health > playerStatus.initialHealth)
        {
            playerStatus.health = playerStatus.initialHealth;
        }
        HealthBar();
    }
}

using UnityEngine;
using UnityEngine.Events;
#pragma warning disable 0649
public class PlayerController : MonoBehaviour //Script ayant pour but de gérer la physique du joueur
{
	[SerializeField] private float m_JumpForce = 400f;                          // Force de saut
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // Valeur servant à rendre le mouvement plus fluide
	[SerializeField] private bool m_AirControl = false;                         // Le joueur peut-il se mouvoir dans les airs ?
	[SerializeField] private LayerMask m_WhatIsGround;                          // Layer correspondant au sol
	[SerializeField] private Transform m_GroundCheck;                           // Marqueur sous les pieds du joueur

	const float k_GroundedRadius = .05f; // Rayon du cercle pour vérifier si le joueur est au sol
	public bool m_Grounded;            // Joueur au sol ?
	public Rigidbody2D m_Rigidbody2D;
	public bool m_FacingRight = true;  // Où le joueur regarde
	private Vector3 m_Velocity = Vector3.zero;

	public UnityEvent OnLandEvent;
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		//Si le marqueur touche un Layer ground, le joueur est considéré au sol
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded && m_Rigidbody2D.velocity.y < 0)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump) //Fonction de mouvement
	{
		//Déplacements possibles si le joueur est au sol ou si le contrôle aérien est activé auquel cas le joueur peut se mouvoir peu importe la situation
		if (m_Grounded || m_AirControl)
		{

			//Mouvement du joueur
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			//Smoothdamp pour rendre le mouvement plus fluide
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// Si on va à droite et qu'on regarde à gauche, on flip
			if (move > 0 && !m_FacingRight)
			{
				Flip();
			}
			//De même si on regarde à droite en se déplaçant à gauche
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
		}
		//Si le joueur doit et peut sauter
		if (m_Grounded && jump)
		{
			//Il n'est plus au sol et une impulsion verticale se produit
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}

	}


	private void Flip()
	{
		//Le joueur regarde dans l'autre sens
		m_FacingRight = !m_FacingRight;

		//Cela se fait en changeant le Scale X par son opposé
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
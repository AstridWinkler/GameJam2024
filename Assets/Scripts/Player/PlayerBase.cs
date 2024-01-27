using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
	[SerializeField]
	protected GameObject particle_jump_up;
	[SerializeField]
	protected GameObject particle_jump_left;
	[SerializeField]
	protected GameObject particle_jump_mural_left;


	public int AnimState { get => anim.GetParameter("idl"); }
	public bool LookingDirection { get => rend.flipX; }

	protected Rigidbody2D rig;
	protected CapsuleCollider2D col;
	protected SpriteRenderer rend;

	protected InputController inputs;

	[SerializeField]
	protected AnimatorRenderer2D_old anim;

	public bool actionKeyPressed { get; protected set; }


	protected virtual void Awake()
	{
		inputs = GameManager.Inputs.GetController();

		rend = GetComponentInChildren<SpriteRenderer>();
		anim = GetComponent<AnimatorRenderer2D_old>();
		rig = GetComponent<Rigidbody2D>();
		col = GetComponent<CapsuleCollider2D>();
		actionKeyPressed = false;

	}


	void Update()
	{
		UpdateAction();
		UpdatePlayer();
	}

	public abstract void UpdatePlayer();



	protected void UpdateAction()
	{

		if (!actionKeyPressed)
			return;

		List<KeyValuePair<I_Interactable, float>> interact = new List<KeyValuePair<I_Interactable, float>>();
		Physics2D.OverlapCircleAll(transform.position, 1.5f, 1 | (1 << 1) | (1 << 10) | (1 << 13)).ToList().ForEach((m) =>
		{
			I_Interactable comp = m.GetComponent<I_Interactable>();
			if (comp != null)
			{
				interact.Add(new KeyValuePair<I_Interactable, float>(comp, Vector2.Distance(transform.position, m.transform.position)));
			}
		});

		interact = interact.Where((m) =>
		{
			return m.Key != null &&
m.Key.CanPlayerAction() &&

		/*Physics2D.Linecast((m.Key as MonoBehaviour).transform.position, transform.position, (1 << 0) | (1 << 9)).transform == null; }).ToList();*/

		Vector3.Distance((m.Key as MonoBehaviour).transform.position, transform.position) < 2f;
		}).ToList();

		interact.Sort((a, b) => Mathf.RoundToInt((a.Value - b.Value) * 100f));
		
	//	Debug.LogError(interact.Count);
		

		if (interact.Count > 0)
		{
		//	Debug.LogError("wouah");
			interact[0].Key.CallPlayerAction(this);
		}


	}



}

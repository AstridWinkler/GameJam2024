using logiked.source.attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Animator2DVariable
{

	[SerializeField]
	private string name;
	[SerializeField]
	[GreyedField]
	private int id;



	public string Name
	{
		get { return name; }
#if UNITY_EDITOR
		set { name = value; }
#endif
	}

	public int UniqueId
	{
		get { return id; }
#if UNITY_EDITOR
		set { id = value; }
#endif
	}



	[SerializeField]
	private int value;


    public int Value
	{
		get { return value; }
		set { this.value = value; }
	}


	public Animator2DVariable(string name, int Id, int value = 0)
	{
		this.name = name;
		this.id = Id;
		this.value = value;
	}

	public Animator2DVariable(Animator2DVariable orig) : this(orig.name, orig.id, orig.value )
	{ }



}

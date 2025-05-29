using System;
namespace Chapeau.Models
{
	public enum Status
	{
		All,// need to be removed 
		New,// ordered
        InProgress,
		Ready,//readyToServe
		Served,
        Completed, //need to be removed 
    }
}


using System;
namespace Chapeau.Models
{
	public enum Status
	{
		All,// need to be removed 
		Ordered,// ordered
        InProgress,
		Ready,//readyToServe
		Served,
        Completed, //need to be removed 
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SecurityDemo.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
	}

	public static class ApplicationRoles
	{

		public const string Admin = "admin";

	}

	public static class ApplicationPolicy {

		public const string BigCheese = "BigCheese";

	}

	public static class ApplicationClaims {

		public const string FavoriteCheese = "favorite_cheese";

	}


}

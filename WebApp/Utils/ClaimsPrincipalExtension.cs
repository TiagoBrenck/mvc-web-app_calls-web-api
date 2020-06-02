using Microsoft.Identity.Client;
using System.Security.Claims;

namespace WebApp.Utils
{
	public static class ClaimsPrincipalExtension
	{
		/// <summary>
		/// Get the Account identifier for an MSAL.NET account from a ClaimsPrincipal
		/// </summary>
		/// <param name="claimsPrincipal">Claims principal</param>
		/// <returns>A string corresponding to an account identifier as defined in <see cref="Microsoft.Identity.Client.AccountId.Identifier"/></returns>
		public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			string userObjectId = GetObjectId(claimsPrincipal);
			string tenantId = GetTenantId(claimsPrincipal);

			if (!string.IsNullOrWhiteSpace(userObjectId) && !string.IsNullOrWhiteSpace(tenantId))
			{
				return $"{userObjectId}.{tenantId}";
			}

			return null;
		}

		/// <summary>
		/// Get the unique object ID associated with the claimsPrincipal
		/// </summary>
		/// <param name="claimsPrincipal">Claims principal from which to retrieve the unique object id</param>
		/// <returns>Unique object ID of the identity, or <c>null</c> if it cannot be found</returns>
		public static string GetObjectId(this ClaimsPrincipal claimsPrincipal)
		{
			var objIdclaim = claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");

			if (objIdclaim == null)
			{
				objIdclaim = claimsPrincipal.FindFirst("oid");
			}

			return objIdclaim != null ? objIdclaim.Value : string.Empty;
		}

		/// <summary>
		/// Tenant ID of the identity
		/// </summary>
		/// <param name="claimsPrincipal">Claims principal from which to retrieve the tenant id</param>
		/// <returns>Tenant ID of the identity, or <c>null</c> if it cannot be found</returns>
		public static string GetTenantId(this ClaimsPrincipal claimsPrincipal)
		{
			var tenantIdclaim = claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");

			if (tenantIdclaim == null)
			{
				tenantIdclaim = claimsPrincipal.FindFirst("tid");
			}

			return tenantIdclaim != null ? tenantIdclaim.Value : string.Empty;
		}

		/// <summary>
		/// Builds a ClaimsPrincipal from an IAccount
		/// </summary>
		/// <param name="account">The IAccount instance.</param>
		/// <returns>A ClaimsPrincipal built from IAccount</returns>
		public static ClaimsPrincipal ToClaimsPrincipal(this IAccount account)
		{
			if (account != null)
			{
				var identity = new ClaimsIdentity();
				identity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", account.HomeAccountId.ObjectId));
				identity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/tenantid", account.HomeAccountId.TenantId));
				identity.AddClaim(new Claim(ClaimTypes.Upn, account.Username));
				return new ClaimsPrincipal(identity);
			}

			return null;
		}

		public static string GetLoginHint(this ClaimsPrincipal claimsPrincipal)
		{
			return GetDisplayName(claimsPrincipal);
		}

		public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
		{
			// Use the claims in a Microsoft identity platform token first
			string displayName = claimsPrincipal.FindFirst("preferred_username")?.Value;

			if (!string.IsNullOrWhiteSpace(displayName))
			{
				return displayName;
			}

			// Otherwise fall back to the claims in an Azure AD v1.0 token
			displayName = claimsPrincipal.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;

			if (!string.IsNullOrWhiteSpace(displayName))
			{
				return displayName;
			}

			// Finally falling back to name
			return claimsPrincipal.FindFirst("name")?.Value;
		}
	}
}
using Zentra.Domain.Entities.Api;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Enums;
using static Zentra.Domain.Constants.Endpoint.OpenIdConstants;

namespace ZentraInstallerMVC.Infrastructure.Seeding;

public static class ZentraMasterDataSeed
{
    public static List<ApiResources> GetApiResourceEntityMaster()
    {
        return
        [
            new ApiResources
            {
                Name = "zentra.apiresource",
                DisplayName = "Zentra.APIRESOURCE",
                Description = "To register and manage ApiResource",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes =
                [
                    new()
                    {
                        Name = "zentra.apiresource.manage", DisplayName = "Zentra.APIRESOURCE.MANAGE",
                        Description = "Manage api resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "zentra.apiresource.read", DisplayName = "Zentra.APIRESOURCE.READ",
                        Description = "Read api resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "zentra.apiresource.write", DisplayName = "Zentra.APIRESOURCE.WRITE",
                        Description = "Write api resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },

                    new()
                    {
                        Name = "zentra.apiresource.delete", DisplayName = "Zentra.APIRESOURCE.DELETE",
                        Description = "Delete api resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                ]
            },

            new()
            {
                Name = "zentra.identityresource",
                DisplayName = "Zentra.IDENTITYRESOURCE",
                Description = "To register and manage IdentityResource",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.identityresource.manage", DisplayName = "Zentra.IDENTITYRESOURCE.MANAGE",
                        Description = "Manage identityresource resource", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.identityresource.read", DisplayName = "Zentra.IDENTITYRESOURCE.READ",
                        Description = "Read identityresource resource", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.identityresource.write", DisplayName = "Zentra.IDENTITYRESOURCE.WRITE",
                        Description = "Write identityresource resource", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.identityresource.delete", DisplayName = "Zentra.IDENTITYRESOURCE.DELETE",
                        Description = "Delete identityresource resource", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "zentra.client",
                DisplayName = "Zentra.CLIENT",
                Description = "To register and manage client",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.client.manage", DisplayName = "Zentra.CLIENT.MANAGE",
                        Description = "Manage client resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.client.read", DisplayName = "Zentra.CLIENT.READ",
                        Description = "Read client resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.client.write", DisplayName = "Zentra.CLIENT.WRITE",
                        Description = "Write client resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.client.delete", DisplayName = "Zentra.CLIENT.DELETE",
                        Description = "Delete client resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "zentra.role",
                DisplayName = "Zentra.ROLE",
                Description = "To register and manage role",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.role.manage", DisplayName = "Zentra.ROLE.MANAGE",
                        Description = "Manage role resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.role.read", DisplayName = "Zentra.ROLE.READ", Description = "Read role resource",
                        CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.role.write", DisplayName = "Zentra.ROLE.WRITE",
                        Description = "Write role resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.role.delete", DisplayName = "Zentra.ROLE.DELETE",
                        Description = "Delete role resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "zentra.adminuser",
                DisplayName = "Zentra.ADMINUSER",
                Description = "To register and manage adminuser",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.adminuser.manage", DisplayName = "Zentra.ADMINUSER.MANAGE",
                        Description = "Manage admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.adminuser.read", DisplayName = "Zentra.ADMINUSER.READ",
                        Description = "Read admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.adminuser.write", DisplayName = "Zentra.ADMINUSER.WRITE",
                        Description = "Write admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.adminuser.delete", DisplayName = "Zentra.ADMINUSER.DELETE",
                        Description = "Delete admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "zentra.user",
                DisplayName = "Zentra.USER",
                Description = "To register and manage user",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.user.manage", DisplayName = "Zentra.USER.MANAGE",
                        Description = "Manage admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.user.read", DisplayName = "Zentra.USER.READ",
                        Description = "Read admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.user.write", DisplayName = "Zentra.USER.WRITE",
                        Description = "Write admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.user.delete", DisplayName = "Zentra.USER.DELETE",
                        Description = "Delete admin resource", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            new()
            {
                Name = "zentra.securitytoken",
                DisplayName = "Zentra.SECURITYTOKEN",
                Description = "Manage Zentra tokens",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "zentra.securitytoken.manage", DisplayName = "Zentra.SECURITYTOKEN.MANAGE",
                        Description = "Manage security token", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    },
                    new()
                    {
                        Name = "zentra.securitytoken.read", DisplayName = "Zentra.SECURITYTOKEN.READ",
                        Description = "Read security token", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "permission", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            },

            // RentFlow app API resource – access tokens include "capabilities" claim from user role
            new()
            {
                Name = "rentflow-api",
                DisplayName = "RentFlow Api",
                Description = "RentFlow app – capabilities in access token by role",
                Enabled = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "ZentraUser",
                ApiResourceClaims = new List<ApiResourceClaims>
                {
                    new()
                    {
                        Type = "capabilities", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                    },
                    new()
                    {
                        Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                    }
                },
                ApiScopes = new List<ApiScopes>
                {
                    new()
                    {
                        Name = "rentflow-api",
                        DisplayName = "RentFlow Api",
                        Description = "RentFlow API access with capabilities claim",
                        CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ApiScopeClaims = new List<ApiScopeClaims>
                        {
                            new()
                            {
                                Type = "capabilities", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                                IsDeleted = false
                            },
                            new()
                            {
                                Type = "role", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow, IsDeleted = false
                            }
                        }
                    }
                }
            }
        ];
    }

    public static List<IdentityResources> CreateIdentityResourceModelMaster()
    {
        return new List<IdentityResources>
        {
            new()
            {
                Name = "openid",
                DisplayName = "OPENID",
                Description = "openid",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Name = "profile",
                DisplayName = "PROFILE",
                Description = "profile",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new()
                    {
                        Type = "subject", CreatedBy = "ZentraUser", AliasType = "sub", CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "name", CreatedBy = "ZentraUser", AliasType = "username", CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "given_name", CreatedBy = "ZentraUser", AliasType = "firstname",
                        CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "family_name", CreatedBy = "ZentraUser", AliasType = "lastname",
                        CreatedOn = DateTime.UtcNow
                    },
                    new() { Type = "middle_name", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "nickname", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "preferred_username", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "profile", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "picture", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "website", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "Gender", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new()
                    {
                        Type = "birthdate", CreatedBy = "ZentraUser", AliasType = "dateofbirth",
                        CreatedOn = DateTime.UtcNow
                    },
                    new() { Type = "zoneinfo", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "locale", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "updated_at", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "City", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "PinCode", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new() { Type = "Street", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow }
                }
            },
            new()
            {
                Name = "email",
                DisplayName = "EMAIL",
                Description = "email",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new() { Type = "email", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow },
                    new()
                    {
                        Type = "email_verified", AliasType = "emailconfirmed", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow
                    }
                }
            },
            new()
            {
                Name = "phone",
                DisplayName = "PHONE",
                Description = "phone",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new()
                    {
                        Type = "phone_number", CreatedBy = "ZentraUser", AliasType = "phonenumber",
                        CreatedOn = DateTime.UtcNow
                    },
                    new()
                    {
                        Type = "phone_number_verified", AliasType = "phonenumberconfirmed", CreatedBy = "ZentraUser",
                        CreatedOn = DateTime.UtcNow
                    }
                }
            },
            new()
            {
                Name = "address",
                DisplayName = "ADDRESS",
                Description = "address",
                Enabled = true,
                Required = true,
                Emphasize = true,
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow,
                IdentityClaims = new List<IdentityClaims>
                {
                    new() { Type = "address", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow }
                }
            }
        };
    }

    public static List<Roles> CreateRolesMaster()
    {
        return new List<Roles>
        {
            new()
            {
                Description = "ZentraAdmin",
                Name = "ZentraAdmin",
                NormalizedName = "ZentraADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },

            new()
            {
                Description = "ZentraUser",
                Name = "ZentraUser",
                NormalizedName = "ZentraUSER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },

            new()
            {
                Description = "RentFlow owner – full tenant and property management",
                Name = "rentflow_owner",
                NormalizedName = "RENTFLOW_OWNER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Description = "RentFlow manager – property and occupancy management",
                Name = "rentflow_manager",
                NormalizedName = "RENTFLOW_MANAGER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Description = "RentFlow resident – limited tenant actions",
                Name = "rentflow_resident",
                NormalizedName = "RENTFLOW_RESIDENT",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            }
        };
    }

    public static List<RoleClaims> CreateRoleClaims_ZentraAdmin()
    {
        var RoleClaimsList = new List<RoleClaims>
        {
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.apiResource.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.identityresource.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.client.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.audittrail.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.role.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.adminuser.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.user.write", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.user.read", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.securitytoken.manage", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            }
        };
        return RoleClaimsList;
    }

    public static List<RoleClaims> CreateRoleClaims_ZentraUser()
    {
        var RoleClaimsList = new List<RoleClaims>
        {
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.role.read", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.user.write", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.user.read", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                ClaimType = "permission", ClaimValue = "zentra.user.delete", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow
            }
        };
        return RoleClaimsList;
    }

    /// <summary>Capabilities for RentFlow owner role – included in access token when scope includes rentflow.</summary>
    public static List<RoleClaims> CreateRoleClaims_RentFlowOwner()
    {
        var capabilities = new[]
        {
            "health:read", "tenant:read", "tenant:write", "tenant:users:read", "tenant:users:invite",
            "tenant:users:accept", "tenant:users:manage", "property:create", "property:read", "property:floor:add",
            "property:room:add", "property:bed:add", "property:spaces:read", "property:spaces:manage",
            "occupancy:read", "occupancy:bed:read", "occupancy:bed:assign", "occupancy:bed:unassign",
            "resident:create", "resident:read", "meal:read"
        };
        return capabilities.Select(c => new RoleClaims
        {
            ClaimType = "capabilities",
            ClaimValue = c,
            CreatedBy = "ZentraUser",
            CreatedOn = DateTime.UtcNow
        }).ToList();
    }

    /// <summary>Capabilities for RentFlow manager role – included in access token when scope includes rentflow.</summary>
    public static List<RoleClaims> CreateRoleClaims_RentFlowManager()
    {
        var capabilities = new[]
        {
            "health:read", "tenant:read", "tenant:users:read", "tenant:users:invite", "property:read",
            "property:floor:add", "property:room:add", "property:bed:add", "property:spaces:read",
            "property:spaces:manage", "occupancy:read", "occupancy:bed:read", "occupancy:bed:assign",
            "occupancy:bed:unassign", "resident:create", "resident:read", "meal:read"
        };
        return capabilities.Select(c => new RoleClaims
        {
            ClaimType = "capabilities",
            ClaimValue = c,
            CreatedBy = "ZentraUser",
            CreatedOn = DateTime.UtcNow
        }).ToList();
    }

    /// <summary>Capabilities for RentFlow resident role – included in access token when scope includes rentflow.</summary>
    public static List<RoleClaims> CreateRoleClaims_RentFlowResident()
    {
        var capabilities = new[] { "health:read", "tenant:users:accept", "meal:skip", "roomchat:read", "roomchat:send" };
        return capabilities.Select(c => new RoleClaims
        {
            ClaimType = "capabilities",
            ClaimValue = c,
            CreatedBy = "ZentraUser",
            CreatedOn = DateTime.UtcNow
        }).ToList();
    }

    public static List<SecurityQuestions> CreateSecurityQuestionsModelMaster()
    {
        return new List<SecurityQuestions>
        {
            new()
            {
                Question = "What primary school did you attend?", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new()
            {
                Question = "In what town or city was your first full time job?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "In what town or city did you meet your spouse or partner?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What are the last five digits of your driver's license number?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What time of the day were you born? (hh:mm)", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What is the name of the place your wedding reception was held?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What is the name of your favorite childhood friend?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            },
            new()
            {
                Question = "What was your childhood nickname?", CreatedBy = "ZentraUser", CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new()
            {
                Question = "What was the last name of your teacher?", CreatedBy = "ZentraUser",
                CreatedOn = DateTime.UtcNow, IsDeleted = false
            }
        };
    }

    public static UserRoles CreateUserRoleModelMaster()
    {
        return new UserRoles
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddDays(90),
            CreatedBy = "ZentraUser",
            CreatedOn = DateTime.UtcNow
        };
    }

    public static Users CreateUserModelMaster()
    {
        return new Users
        {
            TwoFactorEnabled = false,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            RequiresDefaultPasswordChange = false,
            CreatedBy = "ZentraUser",
            CreatedOn = DateTime.UtcNow,
            TwoFactorType = TwoFactorType.None,
            SecurityStamp = "6XPYYMZYMSG73X6CMLERAU4PMQ4W4V6"
        };
    }

    public static Clients CreateClientMaster()
    {
        return new Clients
        {
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "ZentraUser",
            LogoUri = string.Empty,
            TermsOfServiceUri = string.Empty,
            PolicyUri = string.Empty,

            RefreshTokenExpiration = 86400,
            AccessTokenExpiration = 3600,
            IdentityTokenExpiration = 3600,
            AuthorizationCodeExpiration = 1800,
            LogoutTokenExpiration = 1800,

            AccessTokenType = AccessTokenType.JWT,
            RequirePkce = true,
            IsPkceTextPlain = false,
            RequireClientSecret = true,
            IsFirstPartyApp = true,
            AllowAccessTokensViaBrowser = false,

            AllowedSigningAlgorithm = Algorithms.RsaSha256,
            AllowOfflineAccess = true,
            ApplicationType = ApplicationType.RegularWeb,

            FrontChannelLogoutSessionRequired = true,
            BackChannelLogoutSessionRequired = true
        };
    }
}

/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Api;

public class ApiRouteModel
{
    public virtual string Name { get; set; }

    public virtual string Path { get; set; }

    public virtual List<string> Permissions { get; set; }
}

//public class ApiPermissionModel
//{
//    /// <summary>
//    /// Gets or sets the Permission for Api.
//    /// </summary>
//    public virtual string Permission { get; set; }
//}

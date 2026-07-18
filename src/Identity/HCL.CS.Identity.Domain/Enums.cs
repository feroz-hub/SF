/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain;

public enum DbTypes
{
    SqlServer = 1,

    MySql = 2,

    PostgreSQL = 3,

    SQLite = 4
}

public enum WriteLogTo
{
    File = 0,

    DataBase = 1
}

public enum RollingIntervalType
{
    Month = 0,

    Day = 1,

    Hour = 2,

    Minute = 3
}

public enum Log
{
    Debug = 0,

    Error = 1,

    Fatal = 2,

    Information = 3,

    Verbose = 4,

    Warning = 5
}

public enum ResultStatus
{
    Succeeded = 0,

    Failed = 1
}

public enum CrudMode
{
    Add = 0,

    Update = 1,

    Delete = 2
}

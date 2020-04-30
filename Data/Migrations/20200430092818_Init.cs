﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AxGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    OrganizationName = table.Column<string>(nullable: true),
                    OrganizationLogo = table.Column<byte[]>(nullable: true),
                    VersionName = table.Column<string>(nullable: true),
                    VersionDate = table.Column<DateTime>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Logged = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ServerAddress = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(nullable: true),
                    Callsite = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    ShowInMenu = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    AxOp = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    TakeSize = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true),
                    ResultTypeName = table.Column<string>(nullable: true),
                    TakeSize = table.Column<int>(nullable: false),
                    Sort = table.Column<string>(nullable: true),
                    SortType = table.Column<int>(nullable: false),
                    ExecuteType = table.Column<int>(nullable: false),
                    GroupBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    GenderType = table.Column<int>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColumnReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    FieldId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnReports_NewReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "NewReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Property = table.Column<string>(nullable: true),
                    Operation = table.Column<int>(nullable: false),
                    Value1 = table.Column<string>(nullable: true),
                    Value2 = table.Column<string>(nullable: true),
                    Connector = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: true),
                    IsCalculation = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filters_NewReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "NewReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderByReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    OrderIndex = table.Column<int>(nullable: false),
                    Column = table.Column<string>(nullable: true),
                    OrderByType = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderByReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderByReports_NewReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "NewReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AxCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    SystemId = table.Column<int>(nullable: true),
                    ReportId = table.Column<int>(nullable: true),
                    NextChartId = table.Column<int>(nullable: true),
                    NextChartType = table.Column<int>(nullable: true),
                    ChartType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AxCharts_RS_ReportId",
                        column: x => x.ReportId,
                        principalTable: "RS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AxCharts_Menus_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AxFilters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Property = table.Column<string>(nullable: true),
                    Operation = table.Column<int>(nullable: false),
                    Value1 = table.Column<string>(nullable: true),
                    Value2 = table.Column<string>(nullable: true),
                    Connector = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: true),
                    IsCalculation = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxFilters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AxFilters_RS_ReportId",
                        column: x => x.ReportId,
                        principalTable: "RS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Content = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsMainAddress = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    AddressType = table.Column<int>(nullable: false),
                    ExpireDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    PrimaryKey = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    AuditType = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    EntityInsertDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    InvalidPassword = table.Column<string>(nullable: true),
                    Browser = table.Column<string>(nullable: true),
                    BrowserVersion = table.Column<string>(nullable: true),
                    Os = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    AppVersion = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    MachineName = table.Column<string>(nullable: true),
                    ValidSignIn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Access = table.Column<bool>(nullable: false),
                    MenuId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    GroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_AxGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "AxGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permissions_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroups_AxGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "AxGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    DefaultSystemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Menus_DefaultSystemId",
                        column: x => x.DefaultSystemId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    DeviceName = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    Browser = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    ExpireDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BarCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    XField = table.Column<string>(nullable: true),
                    YField = table.Column<string>(nullable: true),
                    AxChartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarCharts_AxCharts_AxChartId",
                        column: x => x.AxChartId,
                        principalTable: "AxCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    AxChartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineCharts_AxCharts_AxChartId",
                        column: x => x.AxChartId,
                        principalTable: "AxCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NumericWidgets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Label = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    AxChartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumericWidgets_AxCharts_AxChartId",
                        column: x => x.AxChartId,
                        principalTable: "AxCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    OrderIndex = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    AxChartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCharts_AxCharts_AxChartId",
                        column: x => x.AxChartId,
                        principalTable: "AxCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCharts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AxSeries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    AxChartId = table.Column<int>(nullable: false),
                    OrderIndex = table.Column<int>(nullable: false),
                    BarChartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AxSeries_BarCharts_BarChartId",
                        column: x => x.BarChartId,
                        principalTable: "BarCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Legends",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    AxChartId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    BarChartId = table.Column<int>(nullable: true),
                    LineChartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Legends_BarCharts_BarChartId",
                        column: x => x.BarChartId,
                        principalTable: "BarCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Legends_LineCharts_LineChartId",
                        column: x => x.LineChartId,
                        principalTable: "LineCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PieCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SeriesId = table.Column<int>(nullable: false),
                    AxChartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieCharts_AxCharts_AxChartId",
                        column: x => x.AxChartId,
                        principalTable: "AxCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PieCharts_AxSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "AxSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_UserId",
                table: "Audits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AxCharts_ReportId",
                table: "AxCharts",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AxCharts_SystemId",
                table: "AxCharts",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_AxFilters_ReportId",
                table: "AxFilters",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_AxSeries_BarChartId",
                table: "AxSeries",
                column: "BarChartId");

            migrationBuilder.CreateIndex(
                name: "IX_BarCharts_AxChartId",
                table: "BarCharts",
                column: "AxChartId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnReports_ReportId",
                table: "ColumnReports",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Filters_ReportId",
                table: "Filters",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Legends_BarChartId",
                table: "Legends",
                column: "BarChartId");

            migrationBuilder.CreateIndex(
                name: "IX_Legends_LineChartId",
                table: "Legends",
                column: "LineChartId");

            migrationBuilder.CreateIndex(
                name: "IX_LineCharts_AxChartId",
                table: "LineCharts",
                column: "AxChartId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginLogs_UserId",
                table: "LoginLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_NumericWidgets_AxChartId",
                table: "NumericWidgets",
                column: "AxChartId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderByReports_ReportId",
                table: "OrderByReports",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_GroupId",
                table: "Permissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_MenuId",
                table: "Permissions",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserId",
                table: "Permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PieCharts_AxChartId",
                table: "PieCharts",
                column: "AxChartId");

            migrationBuilder.CreateIndex(
                name: "IX_PieCharts_SeriesId",
                table: "PieCharts",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharts_AxChartId",
                table: "UserCharts",
                column: "AxChartId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharts_UserId",
                table: "UserCharts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_DefaultSystemId",
                table: "UserSettings",
                column: "DefaultSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "AuditTables");

            migrationBuilder.DropTable(
                name: "AxFilters");

            migrationBuilder.DropTable(
                name: "ColumnReports");

            migrationBuilder.DropTable(
                name: "ConfigData");

            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "Legends");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "NumericWidgets");

            migrationBuilder.DropTable(
                name: "OrderByReports");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PieCharts");

            migrationBuilder.DropTable(
                name: "UserCharts");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "LineCharts");

            migrationBuilder.DropTable(
                name: "NewReports");

            migrationBuilder.DropTable(
                name: "AxSeries");

            migrationBuilder.DropTable(
                name: "AxGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BarCharts");

            migrationBuilder.DropTable(
                name: "AxCharts");

            migrationBuilder.DropTable(
                name: "RS");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addUserFieldsBFT : Migration
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
                name: "FileAttachmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TypeTitle = table.Column<string>(nullable: true),
                    IsBaseType = table.Column<bool>(nullable: false),
                    AttachmentTypeEnum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geos_Geos_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Geos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HardwareDataHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Cpu = table.Column<float>(nullable: false),
                    Ram = table.Column<float>(nullable: false),
                    NetworkIn = table.Column<float>(nullable: false),
                    NetworkOut = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardwareDataHistories", x => x.Id);
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
                name: "Strategies",
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
                    OrderSize = table.Column<decimal>(nullable: false),
                    InitialCapital = table.Column<decimal>(nullable: false),
                    BaseCurrency = table.Column<decimal>(nullable: false),
                    Pyramiding = table.Column<int>(nullable: false),
                    OrderSizeType = table.Column<int>(nullable: false),
                    Commission = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
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
                    Description = table.Column<string>(nullable: true),
                    MonthPrice = table.Column<decimal>(nullable: false),
                    ThreeMonthPrice = table.Column<decimal>(nullable: false),
                    OneYearPrice = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
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
                    ContentBytes = table.Column<byte[]>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Key = table.Column<int>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    TypeName = table.Column<string>(nullable: true),
                    FileAttachmentTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachments_FileAttachmentTypes_FileAttachmentTypeId",
                        column: x => x.FileAttachmentTypeId,
                        principalTable: "FileAttachmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    StrategyId = table.Column<int>(nullable: true),
                    TimeFrames = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisRequests_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StrategyIndicators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    StrategyId = table.Column<int>(nullable: false),
                    IndicatorTypeName = table.Column<string>(nullable: true),
                    ConfigData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategyIndicators_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    UniqueKey = table.Column<string>(nullable: true),
                    FireBaseToken = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    SubscriptionId = table.Column<int>(nullable: true),
                    LoginType = table.Column<int>(nullable: false),
                    ExpireDateTime = table.Column<DateTime>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Symbols",
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
                    Ticker = table.Column<string>(nullable: true),
                    Decimals = table.Column<int>(nullable: false),
                    LotSize = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Logo = table.Column<string>(nullable: true),
                    AnalysisRequestId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Symbols_AnalysisRequests_AnalysisRequestId",
                        column: x => x.AnalysisRequestId,
                        principalTable: "AnalysisRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "Messages",
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
                    Body = table.Column<string>(nullable: true),
                    SeenDateTime = table.Column<DateTime>(nullable: true),
                    IsSeen = table.Column<bool>(nullable: false),
                    SenderId = table.Column<int>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: false),
                    MessageType = table.Column<int>(nullable: false),
                    ReplayId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ReplayId",
                        column: x => x.ReplayId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubscriptionId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    Coin = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    DurationDays = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCharts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "UserMessages",
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
                    Body = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    HaveAttachment = table.Column<bool>(nullable: false),
                    SenderId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Confidentiality = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_SenderId",
                        column: x => x.SenderId,
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
                name: "AnalysisResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    NetProfit = table.Column<decimal>(nullable: false),
                    NetProfitPercentage = table.Column<decimal>(nullable: false),
                    WinRate = table.Column<decimal>(nullable: false),
                    TotalTrades = table.Column<int>(nullable: false),
                    AnalysisRequestId = table.Column<int>(nullable: false),
                    SymbolId = table.Column<int>(nullable: false),
                    TimeFrame = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisResults_AnalysisRequests_AnalysisRequestId",
                        column: x => x.AnalysisRequestId,
                        principalTable: "AnalysisRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalysisResults_Symbols_SymbolId",
                        column: x => x.SymbolId,
                        principalTable: "Symbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMessageReceivers",
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
                    ReceiverType = table.Column<int>(nullable: false),
                    PrimaryKey = table.Column<int>(nullable: false),
                    UserMessageId = table.Column<int>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessageReceivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessageReceivers_UserMessages_UserMessageId",
                        column: x => x.UserMessageId,
                        principalTable: "UserMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    ModifierUserId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ConnectionId = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    UserTokenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnections_UserTokens_UserTokenId",
                        column: x => x.UserTokenId,
                        principalTable: "UserTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisRequests_StrategyId",
                table: "AnalysisRequests",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_AnalysisRequestId",
                table: "AnalysisResults",
                column: "AnalysisRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_SymbolId",
                table: "AnalysisResults",
                column: "SymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_UserId",
                table: "Audits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_FileAttachmentTypeId",
                table: "FileAttachments",
                column: "FileAttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Geos_ParentId",
                table: "Geos",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginLogs_UserId",
                table: "LoginLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplayId",
                table: "Messages",
                column: "ReplayId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

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
                name: "IX_StrategyIndicators_StrategyId",
                table: "StrategyIndicators",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_AnalysisRequestId",
                table: "Symbols",
                column: "AnalysisRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharts_UserId",
                table: "UserCharts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_UserTokenId",
                table: "UserConnections",
                column: "UserTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessageReceivers_UserMessageId",
                table: "UserMessageReceivers",
                column: "UserMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_SenderId",
                table: "UserMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SubscriptionId",
                table: "Users",
                column: "SubscriptionId");

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
                name: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "AuditTables");

            migrationBuilder.DropTable(
                name: "ConfigData");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "Geos");

            migrationBuilder.DropTable(
                name: "HardwareDataHistories");

            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "StrategyIndicators");

            migrationBuilder.DropTable(
                name: "UserCharts");

            migrationBuilder.DropTable(
                name: "UserConnections");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserMessageReceivers");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Symbols");

            migrationBuilder.DropTable(
                name: "FileAttachmentTypes");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "AxGroups");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "AnalysisRequests");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}

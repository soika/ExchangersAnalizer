using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExchangersAnalizer.Migrations
{
    public partial class InitializeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AllowIPs = table.Column<string>(nullable: true),
                    NumberOfCoinsToSend = table.Column<int>(nullable: false),
                    RefreshInMinutes = table.Column<int>(nullable: false),
                    TelegramChatGroups = table.Column<string>(nullable: true),
                    TelegramKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Symbols",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Binance = table.Column<string>(nullable: true),
                    Bittrex = table.Column<string>(nullable: true),
                    Cryptopia = table.Column<string>(nullable: true),
                    GlobalSymbol = table.Column<string>(nullable: true),
                    HitBtc = table.Column<string>(nullable: true),
                    KuCoin = table.Column<string>(nullable: true),
                    Yobit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symbols", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Symbols");
        }
    }
}

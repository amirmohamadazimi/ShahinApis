using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahinApis.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shahin_Req",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LogDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    JsonReq = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PublicAppId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ServiceId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PublicReqId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shahin_Req", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shahin_Res",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ResCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    HTTPStatusCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    JsonRes = table.Column<string>(type: "NCLOB", nullable: true),
                    PublicReqId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ReqLogId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shahin_Res", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shahin_Res_Shahin_Req_ReqLogId",
                        column: x => x.ReqLogId,
                        principalTable: "Shahin_Req",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shahin_Req_Id",
                table: "Shahin_Req",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shahin_Res_Id",
                table: "Shahin_Res",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shahin_Res_ReqLogId",
                table: "Shahin_Res",
                column: "ReqLogId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shahin_Res");

            migrationBuilder.DropTable(
                name: "Shahin_Req");
        }
    }
}

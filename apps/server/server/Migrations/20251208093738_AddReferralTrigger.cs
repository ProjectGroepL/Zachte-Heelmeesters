using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace ZhmApi.Migrations
{
    public partial class AddReferralTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER TRIGGER TR_Referrals_Expire
            ON Referrals
            AFTER INSERT, UPDATE
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Zet ValidUntil indien leeg
                UPDATE r
                SET r.ValidUntil = DATEADD(MONTH, 1, r.CreatedAt)
                FROM Referrals r
                INNER JOIN inserted i ON r.Id = i.Id
                WHERE r.ValidUntil IS NULL OR r.ValidUntil = '0001-01-01';

                -- Zet status op 'verlopen' als ValidUntil gepasseerd is
                UPDATE r
                SET r.Status = 'verlopen'
                FROM Referrals r
                INNER JOIN inserted i ON r.Id = i.Id
                WHERE r.ValidUntil < GETUTCDATE() AND r.Status = 'open';
            END;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_Referrals_Expire;");
        }
    }

}

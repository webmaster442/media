using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Database.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(DatabaseContext))]
    [Migration("20241221113333_ConfigValues")]
    public class ConfigValues : Migration
    {
        private readonly Dictionary<string, string> _values;

        public ConfigValues()
        {
            _values = new()
         {
             { ConfigKeys.FFMpegVersion, new DateTimeOffset().ToString() },
             { ConfigKeys.MpvVersion, new DateTimeOffset().ToString() },
             { ConfigKeys.YtdlpVersion, new DateTimeOffset().ToString() },
             { ConfigKeys.ExternalFfMpegPath, string.Empty },
             { ConfigKeys.ExternalMpvPath, string.Empty },
             { ConfigKeys.ExternalYtdlpPath, string.Empty },
             { ConfigKeys.MpvRemotePort, 12345.ToString() },
             { ConfigKeys.DlnaServerPort, 8085.ToString() },
             { ConfigKeys.ExitOnLaunch, true.ToString() },
             { ConfigKeys.AlwaysOnTop, false.ToString() },
         };
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var value in _values)
            {
                migrationBuilder.Sql($"insert INTO Settings ('Key', 'Value') VALUES ('{value.Key}', '{value.Value}')");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var value in _values)
            {
                migrationBuilder.Sql($"delete from Settings where Settings.Key = '{value.Key}'");
            }
        }
    }
}

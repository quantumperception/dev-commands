using System;
using System.Reflection;
using System.Globalization;

namespace DevCommands
{
    public static class ModInfo
    {
        public const string Name = "Valkyrie.DevCommands";
        public const string Title = "Valkyrie's Dev Commands";
        public const string Group = "Valkyrie";
        public const string Guid = "Valkyrie.ValkyrieNPCs";
        // Version follow Semantic Versioning Scheme (https://semver.org/)
        public const string Version = "1.0.0";
        public const string buildDate = "14-03-2022";
        // Nexus Plugin ID (Use to maintain updates with Nexus)
        // Use GetBuildDate(Assembly.GetExecutingAssembly()); to get build date
        private static DateTime m_GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }

            return default;
        }
        public static String GetBuildDate()
        {
            return ModInfo.buildDate;
            //DateTime buildDate = m_GetBuildDate(Assembly.GetExecutingAssembly());
            //return buildDate.ToString("yyyy-MM-dd");
        }

    }
}

using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Loader;

namespace CSharpToTypeScript.Extensions
{
    public static class TypeExtensions
    {
        const string NullableAttributeTypeName = "System.Runtime.CompilerServices.NullableAttribute";
        const string NullableContextAttributeTypeName = "System.Runtime.CompilerServices.NullableContextAttribute";
        private const string DotNetDllNameSuffix = ", Culture=neutral, PublicKeyToken=adb9793829ddae60";
        private const string DotNetDllSharedPath = @"C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\";

        public static bool IsNullableValueType(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static Type GetNullableValueType(this Type type) => type.GetGenericArguments().Single<Type>();

        public static bool IsNullableReferenceType(this PropertyInfo propertyInfo) => IsNullableReferenceType(propertyInfo.PropertyType, propertyInfo.DeclaringType,
            SafeGetCustomAttributeData(() => propertyInfo.CustomAttributes));

        public static bool IsNullableReferenceType(this FieldInfo fieldInfo) => IsNullableReferenceType(fieldInfo.FieldType, fieldInfo.DeclaringType,
            SafeGetCustomAttributeData(() => fieldInfo.CustomAttributes));

        public static bool IsNullableReferenceType(this ParameterInfo parameterInfo) => IsNullableReferenceType(parameterInfo.ParameterType, null,
            SafeGetCustomAttributeData(() => parameterInfo.CustomAttributes));

        public static T? SafeGetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            try
            {
                return memberInfo.GetCustomAttribute<T>(inherit);
            }
            catch (FileNotFoundException ex)
            {
                if (ex.FileName?.EndsWith(DotNetDllNameSuffix) == true)
                {
                    try
                    {
                        var sharedFilePath = Path.Combine(DotNetDllSharedPath, GetDllVersionedPath(ex.FileName));
                        Assembly aspnetcoreAssembly = Assembly.LoadFrom(sharedFilePath);
                        return memberInfo.GetCustomAttribute<T>(inherit);
                    }
                    catch
                    {
                        Console.WriteLine(ex.Message);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static IEnumerable<T> SafeGetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            try
            {
                return memberInfo.GetCustomAttributes<T>(inherit);
            }
            catch (FileNotFoundException ex)
            {
                if (ex.FileName?.EndsWith(DotNetDllNameSuffix) == true)
                {
                    try
                    {
                        var sharedFilePath = Path.Combine(DotNetDllSharedPath, GetDllVersionedPath(ex.FileName));
                        Assembly aspnetcoreAssembly = Assembly.LoadFrom(sharedFilePath);
                        return memberInfo.GetCustomAttributes<T>(inherit);
                    }
                    catch
                    {
                        Console.WriteLine(ex.Message);
                        return Array.Empty<T>();
                    }
                }
                else
                {
                    return Array.Empty<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Array.Empty<T>();
            }
        }

        private static IEnumerable<CustomAttributeData> SafeGetCustomAttributeData(Func<IEnumerable<CustomAttributeData>> getCustomAttrData)
        {
            IEnumerable<CustomAttributeData> customAttributeData;
            try
            {
                customAttributeData = getCustomAttrData();
            }
            catch (FileNotFoundException ex)
            {
                if (ex.FileName?.EndsWith(DotNetDllNameSuffix) == true)
                {
                    try
                    {
                        var sharedFilePath = Path.Combine(DotNetDllSharedPath, GetDllVersionedPath(ex.FileName));
                        Assembly aspnetcoreAssembly = Assembly.LoadFrom(sharedFilePath);
                        customAttributeData = getCustomAttrData();
                    }
                    catch
                    {
                        Console.WriteLine(ex.Message);
                        customAttributeData = Array.Empty<CustomAttributeData>();
                    }
                }
                else
                {
                    customAttributeData = Array.Empty<CustomAttributeData>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                customAttributeData = Array.Empty<CustomAttributeData>();
            }

            return customAttributeData;
        }

        private static string GetDllVersionedPath(string fileName)
        {
            var versionedFileName = fileName.Substring(0, fileName.Length - DotNetDllNameSuffix.Length).TrimEnd();
            var versionedSeparatorIndex = versionedFileName.LastIndexOf(',');
            var dllFileName = versionedFileName.Substring(0, versionedSeparatorIndex).TrimEnd();
            var version = versionedFileName.Substring(versionedSeparatorIndex +1).TrimStart().Substring("Version=".Length);
            var minorBuildIndex = version.LastIndexOf('.');
            version = version.Substring(0, minorBuildIndex);

            if (Path.Exists(Path.Combine(DotNetDllSharedPath, version)))
            {
                return Path.Combine(version, dllFileName + ".dll");
            }


            var dllVersion = new DllVersionInfo(version);
            List<DllVersionInfo> versions = new(); 
            foreach (var versionDirectory in Directory.GetDirectories(DotNetDllSharedPath))
            { 
                var directoryVersion = Path.GetFileName(versionDirectory);
                var directoryVersionInfo = new DllVersionInfo(directoryVersion);
                if (directoryVersionInfo > dllVersion)
                {
                    versions.Add(directoryVersionInfo);
                }
            }

            if (versions.Count == 0)
            {
                return Path.Combine(version, dllFileName + ".dll");
            }

            return Path.Combine(versions.Min(DllVersionInfo.Comparer)!.ToString()!, dllFileName + ".dll");
        }

        private static bool IsNullableReferenceType(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (memberType.IsValueType)
                return false;

            var nullable = customAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeTypeName);
            if (nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if (attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value! == 2;
                    }
                }
                else if (attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value! == 2;
                }
            }

            for (var type = declaringType; type != null; type = type.DeclaringType)
            {
                var context = type.CustomAttributes
                    .FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeTypeName);
                if (context != null &&
                    context.ConstructorArguments.Count == 1 &&
                    context.ConstructorArguments[0].ArgumentType == typeof(byte))
                {
                    return (byte)context.ConstructorArguments[0].Value! == 2;
                }
            }

            return false;
        }
    }
}

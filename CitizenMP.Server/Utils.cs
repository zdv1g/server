﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using MsgPack.Serialization;

namespace CitizenMP.Server
{
    static class Utils
    {
        public static Dictionary<string, string> ParseQueryString(String query)
        {
            Dictionary<String, String> queryDict = new Dictionary<string, string>();
            foreach (String token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    queryDict[parts[0].Trim()] = parts[1].Trim();
                else
                    queryDict[parts[0].Trim()] = "";
            }
            return queryDict;
        }

        public static string GetFileSHA1String(string filename)
        {
            using (var baseStream = File.OpenRead(filename))
            {
                using (var stream = new BufferedStream(baseStream, 32768))
                {
                    using (var sha = SHA1.Create())
                    {
                        var hash = sha.ComputeHash(stream);
                        var sb = new StringBuilder(2 * hash.Length);
                        
                        foreach (var b in hash)
                        {
                            sb.AppendFormat("{0:X2}", b);
                        }

                        return sb.ToString();
                    }
                }
            }
        }

        public static string[] Tokenize(string text)
        {
            int i = 0;
            int j = 0;
            string[] args = new string[0];

            while (true)
            {
                // skip whitespace and comments and such
                while (true)
                {
                    // skip whitespace and control characters
                    while (i < text.Length && text[i] <= ' ')
                    {
                        i++;
                    }

                    if (i >= text.Length)
                    {
                        return args;
                    }

                    // hopefully this will fix some errors
                    if (i == 0)
                    {
                        break;
                    }

                    // skip comments
                    if (text[i] == '/' && text[i + 1] == '/')
                    {
                        return args;
                    }

                    // /* comments
                    if (text[i] == '/' && text[i + 1] == '*')
                    {
                        while (i < (text.Length - 1) && (text[i] != '*' || text[i + 1] != '/'))
                        {
                            i++;
                        }

                        if (i >= text.Length)
                        {
                            return args;
                        }

                        i += 2;
                    }
                    else
                    {
                        break;
                    }
                }

                Array.Resize(ref args, args.Length + 1);

                StringBuilder arg = new StringBuilder();

                // quoted strings
                if (text[i] == '"')
                {
                    bool inEscape = false;

                    while (true)
                    {
                        i++;

                        if (i >= text.Length)
                        {
                            break;
                        }

                        if (text[i] == '"' && !inEscape)
                        {
                            break;
                        }

                        if (text[i] == '\\')
                        {
                            inEscape = true;
                        }
                        else
                        {
                            arg.Append(text[i]);
                            inEscape = false;
                        }
                    }

                    i++;

                    args[j] = arg.ToString();
                    j++;

                    if (i >= text.Length)
                    {
                        return args;
                    }

                    continue;
                }

                // non-quoted strings
                while (i < text.Length && text[i] > ' ')
                {
                    if (text[i] == '"')
                    {
                        break;
                    }

                    if (i < (text.Length - 1))
                    {
                        if (text[i] == '/' && text[i + 1] == '/')
                        {
                            break;
                        }

                        if (text[i] == '/' && text[i + 1] == '*')
                        {
                            break;
                        }
                    }

                    arg.Append(text[i]);

                    i++;
                }

                args[j] = arg.ToString();
                j++;

                if (i >= text.Length)
                {
                    return args;
                }
            }
        }

        static Utils()
        {
            SerializationContext.Default.ResolveSerializer += (sender, args) =>
            {
                if (args.TargetType.IsSubclassOf(typeof(Delegate)))
                {
                    // probably *really* slow, but this is the only way it seems to be possible with the strict generic type requirements
                    var instance = Activator.CreateInstance(typeof(DelegateSerializer<>).MakeGenericType(args.TargetType));
                    args.GetType().GetMethod("SetSerializer").MakeGenericMethod(args.TargetType).Invoke(args, new[] { instance });
                }
            };
        }

        public static byte[] SerializeEvent(object[] args)
        {
            var stream = new MemoryStream();
            var packer = MsgPack.Packer.Create(stream, MsgPack.PackerCompatibilityOptions.PackBinaryAsRaw);

            packer.PackArrayHeader(args.Length);

            for (int i = 0; i < args.Length; i++)
            {
                var mpo = MessagePackSerializer.Get(args[i].GetType());
                mpo.PackTo(packer, args[i]);
            }

            // make it into a string for lua
            return stream.ToArray();
        }
    }

    class DelegateSerializer<T> : MessagePackSerializer<T>
    {
        public DelegateSerializer()
            : base(SerializationContext.Default)
        {

        }

        protected override void PackToCore(MsgPack.Packer packer, T objectTree)
        {
            var index = Resources.InternalCallRefHandler.Get().AddCallback(objectTree as Delegate);

            var resourceNameBytes = Encoding.UTF8.GetBytes("__internal");
            var delegateData = new byte[8 + resourceNameBytes.Length];

            Array.Copy(BitConverter.GetBytes(index).Reverse().ToArray(), 0, delegateData, 0, 4);
            Array.Copy(BitConverter.GetBytes(0).Reverse().ToArray(), 0, delegateData, 4, 4);
            Array.Copy(resourceNameBytes, 0, delegateData, 8, resourceNameBytes.Length);

            packer.PackExtendedTypeValue(1, delegateData);
        }

        protected override T UnpackFromCore(MsgPack.Unpacker unpacker)
        {
            throw new NotImplementedException();
        }
    }
}

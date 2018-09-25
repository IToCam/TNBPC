using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace BPC.Authenticate
{
    public static class CredentialManager
    {
        public static Authenticate.Credential ReadCredential(string applicationName)
        {
            IntPtr nCredPtr;
            bool read = CredRead(applicationName, CredentialType.Generic, 0, out nCredPtr);
            if (read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr))
                {
                    Credential cred = critCred.GetCredential();
                    return ReadCredential(cred);
                }
            }

            return null;
        }

        private static Authenticate.Credential ReadCredential(Credential credential)
        {
            string applicationName = Marshal.PtrToStringUni(credential.TargetName);
            string userName = Marshal.PtrToStringUni(credential.UserName);
            string secret = null;
            if (credential.CredentialBlob != IntPtr.Zero)
            {
                secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);
            }

            return new Authenticate.Credential( applicationName, userName, secret);
        }
        
        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        static extern bool CredFree([In] IntPtr cred);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct Credential
        {
            public UInt32 Flags;
            public CredentialType Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
        {
            public CriticalCredentialHandle(IntPtr preexistingHandle)
            {
                SetHandle(preexistingHandle);
            }

            public Credential GetCredential()
            {
                if (!IsInvalid)
                {
                    Credential credential = (Credential)Marshal.PtrToStructure(handle, typeof(Credential));
                    return credential;
                }

                throw new InvalidOperationException("Invalid CriticalHandle!");
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    CredFree(handle);
                    SetHandleAsInvalid();
                    return true;
                }

                return false;
            }
        }
    }

    public enum CredentialType
    {
        Generic = 1,
        DomainPassword,
        DomainCertificate,
        DomainVisiblePassword,
        GenericCertificate,
        DomainExtended,
        Maximum,
        MaximumEx = Maximum + 1000,
    }

    public class Credential
    {
        public string ApplicationId { get; }

        public string UserName { get; }

        public string Password { get; }

        public Credential(string applicationId, string userName, string password)
        {
            ApplicationId = applicationId;
            UserName = userName;
            Password = password;
        }

        public override string ToString()
        {
            return $"ApplicationName: {ApplicationId}, UserName: {UserName}";
        }
    }
}

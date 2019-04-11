﻿using System;
using System.IO;
using System.Linq;
using NDecrypt.Data;

namespace NDecrypt.Headers
{
    public class NDSHeader
    {
        #region Encryption process variables

        private uint[] cardHash = new uint[0x412];
        private uint[] arg2 = new uint[3];

        // ARM9 decryption check values
        private const uint MAGIC30 = 0x72636E65;
        private const uint MAGIC34 = 0x6A624F79;

        #endregion

        #region Common to all NDS files

        /// <summary>
        /// Game Title
        /// </summary>
        public char[] GameTitle { get; private set; }

        /// <summary>
        /// Gamecode
        /// </summary>
        public uint Gamecode { get; private set; }

        /// <summary>
        /// Makercode
        /// </summary>
        public char[] Makercode { get; private set; }

        /// <summary>
        /// Unitcode
        /// </summary>
        public NDSUnitcode Unitcode { get; private set; }

        /// <summary>
        /// Encryption seed select (device code. 0 = normal)
        /// </summary>
        public byte EncryptionSeedSelect { get; private set; }

        /// <summary>
        /// Devicecapacity
        /// </summary>
        public byte Devicecapacity { get; private set; }
        public int DeviceCapacityInBytes { get { return (1 << Devicecapacity) * (1024 * 1024); } }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1 { get; private set; }

        /// <summary>
        /// Game Revision (used by DSi titles)
        /// </summary>
        public ushort GameRevision { get; private set; }

        /// <summary>
        /// ROM Version
        /// </summary>
        public byte RomVersion { get; private set; }

        /// <summary>
        /// Internal flags, (Bit2: Autostart)
        /// </summary>
        public byte InternalFlags { get; private set; }

        /// <summary>
        /// ARM9 rom offset
        /// </summary>
        public uint ARM9RomOffset { get; private set; }

        /// <summary>
        /// ARM9 entry address
        /// </summary>
        public uint ARM9EntryAddress { get; private set; }

        /// <summary>
        /// ARM9 load address
        /// </summary>
        public uint ARM9LoadAddress { get; private set; }

        /// <summary>
        /// ARM9 size
        /// </summary>
        public uint ARM9Size { get; private set; }

        /// <summary>
        /// ARM7 rom offset
        /// </summary>
        public uint ARM7RomOffset { get; private set; }

        /// <summary>
        /// ARM7 entry address
        /// </summary>
        public uint ARM7EntryAddress { get; private set; }

        /// <summary>
        /// ARM7 load address
        /// </summary>
        public uint ARM7LoadAddress { get; private set; }

        /// <summary>
        /// ARM7 size
        /// </summary>
        public uint ARM7Size { get; private set; }

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint FileNameTableOffset { get; private set; }

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint FileNameTableLength { get; private set; }

        /// <summary>
        /// File Allocation Table (FNT) offset
        /// </summary>
        public uint FileAllocationTableOffset { get; private set; }

        /// <summary>
        /// File Allocation Table (FNT) length
        /// </summary>
        public uint FileAllocationTableLength { get; private set; }

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint ARM9OverlayOffset { get; private set; }

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint ARM9OverlayLength { get; private set; }

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint ARM7OverlayOffset { get; private set; }

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint ARM7OverlayLength { get; private set; }

        /// <summary>
        /// Normal card control register settings (0x00416657 for OneTimePROM)
        /// </summary>
        public byte[] NormalCardControlRegisterSettings { get; private set; }

        /// <summary>
        /// Secure card control register settings (0x081808F8 for OneTimePROM)
        /// </summary>
        public byte[] SecureCardControlRegisterSettings { get; private set; }

        /// <summary>
        /// Icon Banner offset (NDSi same as NDS, but with new extra entries)
        /// </summary>
        public uint IconBannerOffset { get; private set; }

        /// <summary>
        /// Secure area (2K) CRC
        /// </summary>
        public ushort SecureAreaCRC { get; private set; }

        /// <summary>
        /// Secure transfer timeout (0x0D7E for OneTimePROM)
        /// </summary>
        public ushort SecureTransferTimeout { get; private set; }

        /// <summary>
        /// ARM9 autoload
        /// </summary>
        public byte[] ARM9Autoload { get; private set; }

        /// <summary>
        /// ARM7 autoload
        /// </summary>
        public byte[] ARM7Autoload { get; private set; }

        /// <summary>
        /// Secure disable
        /// </summary>
        public byte[] SecureDisable { get; private set; }

        /// <summary>
        /// NTR region ROM size (excluding DSi area)
        /// </summary>
        public uint NTRRegionRomSize { get; private set; }

        /// <summary>
        /// Header size
        /// </summary>
        public uint HeaderSize { get; private set; }

        /// <summary>
        ///Reserved (0x88, 0x8C, 0x90 = Unknown, used by DSi)
        /// </summary>
        public byte[] Reserved2 { get; private set; }

        /// <summary>
        /// Nintendo Logo
        /// </summary>
        public byte[] NintendoLogo { get; private set; }

        /// <summary>
        /// Nintendo Logo CRC
        /// </summary>
        public ushort NintendoLogoCRC { get; private set; }

        /// <summary>
        /// Header CRC
        /// </summary>
        public ushort HeaderCRC { get; private set; }

        /// <summary>
        /// Debugger reserved
        /// </summary>
        public byte[] DebuggerReserved { get; private set; }

        #endregion

        #region Extended DSi

        /// <summary>
        /// Global MBK1..MBK5 Settings
        /// </summary>
        public byte[] GlobalMBK15Settings { get; private set; }

        /// <summary>
        ///	Local MBK6..MBK8 Settings for ARM9
        /// </summary>
        public byte[] LocalMBK68SettingsARM9 { get; private set; }

        /// <summary>
        /// Local MBK6..MBK8 Settings for ARM7
        /// </summary>
        public byte[] LocalMBK68SettingsARM7 { get; private set; }

        /// <summary>
        /// Global MBK9 Setting
        /// </summary>
        public byte[] GlobalMBK9Setting { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] RegionFlags { get; private set; }

        /// <summary>
        /// Access control
        /// </summary>
        public byte[] AccessControl { get; private set; }

        /// <summary>
        /// ARM7 SCFG EXT mask (controls which devices to enable)
        /// </summary>
        public byte[] ARM7SCFGEXTMask { get; private set; }

        /// <summary>
        /// Reserved/flags? When bit2 of byte 0x1bf is set, usage of banner.sav from the title data dir is enabled.(additional banner data)
        /// </summary>
        public byte[] ReservedFlags { get; private set; }

        /// <summary>
        /// ARM9i rom offset
        /// </summary>
        public uint ARM9iRomOffset { get; private set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved3 { get; private set; }

        /// <summary>
        /// ARM9i load address
        /// </summary>
        public uint ARM9iLoadAddress { get; private set; }

        /// <summary>
        /// ARM9i size;
        /// </summary>
        public uint ARM9iSize { get; private set; }

        /// <summary>
        /// ARM7i rom offset
        /// </summary>
        public uint ARM7iRomOffset { get; private set; }

        /// <summary>
        /// Pointer to base address where various structures and parameters are passed to the title - what is that???
        /// </summary>
        public byte[] Reserved4 { get; private set; }

        /// <summary>
        /// ARM7i load address
        /// </summary>
        public uint ARM7iLoadAddress { get; private set; }

        /// <summary>
        /// ARM7i size;
        /// </summary>
        public uint ARM7iSize { get; private set; }

        /// <summary>
        /// Digest NTR region offset
        /// </summary>
        public uint DigestNTRRegionOffset { get; private set; }

        /// <summary>
        /// Digest NTR region length
        /// </summary>
        public uint DigestNTRRegionLength { get; private set; }

        // <summary>
        /// Digest TWL region offset
        /// </summary>
        public uint DigestTWLRegionOffset { get; private set; }

        /// <summary>
        /// Digest TWL region length
        /// </summary>
        public uint DigestTWLRegionLength { get; private set; }

        // <summary>
        /// Digest Sector Hashtable region offset
        /// </summary>
        public uint DigestSectorHashtableRegionOffset { get; private set; }

        /// <summary>
        /// Digest Sector Hashtable region length
        /// </summary>
        public uint DigestSectorHashtableRegionLength { get; private set; }

        // <summary>
        /// Digest Block Hashtable region offset
        /// </summary>
        public uint DigestBlockHashtableRegionOffset { get; private set; }

        /// <summary>
        /// Digest Block Hashtable region length
        /// </summary>
        public uint DigestBlockHashtableRegionLength { get; private set; }

        /// <summary>
        /// Digest Sector size
        /// </summary>
        public uint DigestSectorSize { get; private set; }

        /// <summary>
        /// Digeset Block Sectorount
        /// </summary>
        public uint DigestBlockSectorCount { get; private set; }

        /// <summary>
        /// Icon Banner Size (usually 0x23C0)
        /// </summary>
        public uint IconBannerSize { get; private set; }

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public byte[] Unknown1 { get; private set; }

        /// <summary>
        /// NTR+TWL region ROM size (total size including DSi area)
        /// </summary>
        public uint NTRTWLRegionRomSize { get; private set; }

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public byte[] Unknown2 { get; private set; }

        /// <summary>
        /// Modcrypt area 1 offset
        /// </summary>
        public uint ModcryptArea1Offset { get; private set; }

        /// <summary>
        /// Modcrypt area 1 size
        /// </summary>
        public uint ModcryptArea1Size { get; private set; }

        /// <summary>
        /// Modcrypt area 2 offset
        /// </summary>
        public uint ModcryptArea2Offset { get; private set; }

        /// <summary>
        /// Modcrypt area 2 size
        /// </summary>
        public uint ModcryptArea2Size { get; private set; }

        /// <summary>
        /// Title ID
        /// </summary>
        public byte[] TitleID { get; private set; }

        /// <summary>
        /// DSiWare: "public.sav" size
        /// </summary>
        public uint DSiWarePublicSavSize { get; private set; }

        /// <summary>
        /// DSiWare: "private.sav" size
        /// </summary>
        public uint DSiWarePrivateSavSize { get; private set; }

        /// <summary>
        /// Reserved (zero)
        /// </summary>
        public byte[] ReservedZero { get; private set; }

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public byte[] Unknown3 { get; private set; }

        /// <summary>
        /// ARM9 (with encrypted secure area) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9WithSecureAreaSHA1HMACHash { get; private set; }

        /// <summary>
        /// ARM7 SHA1 HMAC hash
        /// </summary>
        public byte[] ARM7SHA1HMACHash { get; private set; }

        /// <summary>
        /// Digest master SHA1 HMAC hash
        /// </summary>
        public byte[] DigestMasterSHA1HMACHash { get; private set; }

        /// <summary>
        /// Banner SHA1 HMAC hash
        /// </summary>
        public byte[] BannerSHA1HMACHash { get; private set; }

        /// <summary>
        /// ARM9i (decrypted) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9iDecryptedSHA1HMACHash { get; private set; }

        /// <summary>
        /// ARM7i (decrypted) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM7iDecryptedSHA1HMACHash { get; private set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved5 { get; private set; }

        /// <summary>
        /// ARM9 (without secure area) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9NoSecureAreaSHA1HMACHash { get; private set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved6 { get; private set; }

        /// <summary>
        /// Reserved and unchecked region, always zero. Used for passing arguments in debug environment.
        /// </summary>
        public byte[] ReservedAndUnchecked { get; private set; }

        /// <summary>
        /// RSA signature (the first 0xE00 bytes of the header are signed with an 1024-bit RSA signature).
        /// </summary>
        public byte[] RSASignature { get; private set; }

        #endregion

        /// <summary>
        /// Read from a stream and get an NDS/NDSi header, if possible
        /// </summary>
        /// <param name="reader">BinaryReader representing the input stream</param>
        /// <returns>NDS/NDSi header object, null on error</returns>
        public static NDSHeader Read(BinaryReader reader)
        {
            NDSHeader header = new NDSHeader();

            try
            {
                header.GameTitle = reader.ReadChars(0xC);
                header.Gamecode = reader.ReadUInt32();
                header.Makercode = reader.ReadChars(2);
                header.Unitcode = (NDSUnitcode)reader.ReadByte();
                header.EncryptionSeedSelect = reader.ReadByte();
                header.Devicecapacity = reader.ReadByte();
                header.Reserved1 = reader.ReadBytes(7);
                header.GameRevision = reader.ReadUInt16();
                header.RomVersion = reader.ReadByte();
                header.InternalFlags = reader.ReadByte();
                header.ARM9RomOffset = reader.ReadUInt32();
                header.ARM9EntryAddress = reader.ReadUInt32();
                header.ARM9LoadAddress = reader.ReadUInt32();
                header.ARM9Size = reader.ReadUInt32();
                header.ARM7RomOffset = reader.ReadUInt32();
                header.ARM7EntryAddress = reader.ReadUInt32();
                header.ARM7LoadAddress = reader.ReadUInt32();
                header.ARM7Size = reader.ReadUInt32();
                header.FileNameTableOffset = reader.ReadUInt32();
                header.FileNameTableLength = reader.ReadUInt32();
                header.FileAllocationTableOffset = reader.ReadUInt32();
                header.FileAllocationTableLength = reader.ReadUInt32();
                header.ARM9OverlayOffset = reader.ReadUInt32();
                header.ARM9OverlayLength = reader.ReadUInt32();
                header.ARM7OverlayOffset = reader.ReadUInt32();
                header.ARM7OverlayLength = reader.ReadUInt32();
                header.SecureDisable = reader.ReadBytes(8);
                header.NTRRegionRomSize = reader.ReadUInt32();
                header.HeaderSize = reader.ReadUInt32();
                header.Reserved2 = reader.ReadBytes(56);
                header.NintendoLogo = reader.ReadBytes(156);
                header.NintendoLogoCRC = reader.ReadUInt16();
                header.DebuggerReserved = reader.ReadBytes(0x20);

                // If we have a DSi compatible title
                if (header.Unitcode == NDSUnitcode.NDSPlusDSi
                    || header.Unitcode == NDSUnitcode.DSi)
                {
                    header.GlobalMBK15Settings = reader.ReadBytes(20);
                    header.LocalMBK68SettingsARM9 = reader.ReadBytes(12);
                    header.LocalMBK68SettingsARM7 = reader.ReadBytes(12);
                    header.GlobalMBK9Setting = reader.ReadBytes(4);
                    header.RegionFlags = reader.ReadBytes(4);
                    header.AccessControl = reader.ReadBytes(4);
                    header.ARM7SCFGEXTMask = reader.ReadBytes(4);
                    header.ReservedFlags = reader.ReadBytes(4);
                    header.ARM9iRomOffset = reader.ReadUInt32();
                    header.Reserved3 = reader.ReadBytes(4);
                    header.ARM9iLoadAddress = reader.ReadUInt32();
                    header.ARM9iSize = reader.ReadUInt32();
                    header.ARM7iRomOffset = reader.ReadUInt32();
                    header.Reserved4 = reader.ReadBytes(4);
                    header.ARM7iLoadAddress = reader.ReadUInt32();
                    header.ARM7iSize = reader.ReadUInt32();
                    header.DigestNTRRegionOffset = reader.ReadUInt32();
                    header.DigestNTRRegionLength = reader.ReadUInt32();
                    header.DigestTWLRegionOffset = reader.ReadUInt32();
                    header.DigestTWLRegionLength = reader.ReadUInt32();
                    header.DigestSectorHashtableRegionOffset = reader.ReadUInt32();
                    header.DigestSectorHashtableRegionLength = reader.ReadUInt32();
                    header.DigestBlockHashtableRegionOffset = reader.ReadUInt32();
                    header.DigestBlockHashtableRegionLength = reader.ReadUInt32();
                    header.DigestSectorSize = reader.ReadUInt32();
                    header.DigestBlockSectorCount = reader.ReadUInt32();
                    header.IconBannerSize = reader.ReadUInt32();
                    header.Unknown1 = reader.ReadBytes(4);
                    header.ModcryptArea1Offset = reader.ReadUInt32();
                    header.ModcryptArea1Size = reader.ReadUInt32();
                    header.ModcryptArea2Offset = reader.ReadUInt32();
                    header.ModcryptArea2Size = reader.ReadUInt32();
                    header.TitleID = reader.ReadBytes(8);
                    header.DSiWarePublicSavSize = reader.ReadUInt32();
                    header.DSiWarePrivateSavSize = reader.ReadUInt32();
                    header.ReservedZero = reader.ReadBytes(176);
                    header.Unknown2 = reader.ReadBytes(0x10);
                    header.ARM9WithSecureAreaSHA1HMACHash = reader.ReadBytes(20);
                    header.ARM7SHA1HMACHash = reader.ReadBytes(20);
                    header.DigestMasterSHA1HMACHash = reader.ReadBytes(20);
                    header.BannerSHA1HMACHash = reader.ReadBytes(20);
                    header.ARM9iDecryptedSHA1HMACHash = reader.ReadBytes(20);
                    header.ARM7iDecryptedSHA1HMACHash = reader.ReadBytes(20);
                    header.Reserved5 = reader.ReadBytes(40);
                    header.ARM9NoSecureAreaSHA1HMACHash = reader.ReadBytes(20);
                    header.Reserved6 = reader.ReadBytes(2636);
                    header.ReservedAndUnchecked = reader.ReadBytes(0x180);
                    header.RSASignature = reader.ReadBytes(0x80);
                }

                return header;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Process secure area in the DS/DSi file
        /// </summary>
        /// <param name="reader">BinaryReader representing the input stream</param>
        /// <param name="writer">BinaryWriter representing the output stream</param>
        /// <param name="encrypt">True if we want to encrypt the partitions, false otherwise</param>
        public void ProcessSecureArea(BinaryReader reader, BinaryWriter writer, bool encrypt)
        {
            bool isDecrypted = CheckIfDecrypted(reader);
            if (encrypt ^ isDecrypted)
            {
                Console.WriteLine("File is already " + (encrypt ? "encrypted" : "decrypted"));
                return;
            }

            ProcessARM9(reader, writer, encrypt);

            Console.WriteLine("File has been " + (encrypt ? "encrypted" : "decrypted"));
        }

        /// <summary>
        /// Determine if the current file is already decrypted or not
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private bool CheckIfDecrypted(BinaryReader reader)
        {
            reader.BaseStream.Seek(0x4000, SeekOrigin.Begin);
            uint firstValue = reader.ReadUInt32();
            uint secondValue = reader.ReadUInt32();

            return ((firstValue == 0xE7FFDEFF) && (secondValue == 0xE7FFDEFF))
                || ((firstValue == 0xD0D48B67) && (secondValue == 0x39392F23)); // Edge case for a couple of items
        }

        /// <summary>
        /// Process the secure ARM9 region of the file, if possible
        /// </summary>
        /// <param name="reader">BinaryReader representing the input stream</param>
        /// <param name="writer">BinaryWriter representing the output stream</param>
        /// <param name="encrypt">True if we want to encrypt the partitions, false otherwise</param>
        private void ProcessARM9(BinaryReader reader, BinaryWriter writer, bool encrypt)
        {
            // Seek to the beginning of the secure area
            reader.BaseStream.Seek(0x4000, SeekOrigin.Begin);
            writer.BaseStream.Seek(0x4000, SeekOrigin.Begin);

            // Grab the first two blocks
            uint p0 = reader.ReadUInt32();
            uint p1 = reader.ReadUInt32();

            // Perform the initialization steps
            Init1();
            if (!encrypt) Decrypt(ref p1, ref p0);
            arg2[1] <<= 1;
            arg2[2] >>= 1;
            Init2();

            // If we're decrypting, set the proper flags
            if (!encrypt)
            {
                Decrypt(ref p1, ref p0);
                if (p0 == MAGIC30 && p1 == MAGIC34)
                {
                    p0 = 0xE7FFDEFF;
                    p1 = 0xE7FFDEFF;
                }

                writer.Write(p0);
                writer.Write(p1);
            }

            // Ensure alignment
            reader.BaseStream.Seek(0x4008, SeekOrigin.Begin);
            writer.BaseStream.Seek(0x4008, SeekOrigin.Begin);

            // Loop throgh the main encryption step
            uint size = 0x800 - 8;
            while (size > 0)
            {
                p0 = reader.ReadUInt32();
                p1 = reader.ReadUInt32();

                if (encrypt)
                    Encrypt(ref p1, ref p0);
                else
                    Decrypt(ref p1, ref p0);

                writer.Write(p0);
                writer.Write(p1);

                size -= 8;
            }

            // Replace the header explicitly if we're encrypting
            if (encrypt)
            {
                reader.BaseStream.Seek(0x4000, SeekOrigin.Begin);
                writer.BaseStream.Seek(0x4000, SeekOrigin.Begin);

                p0 = reader.ReadUInt32();
                p1 = reader.ReadUInt32();

                if (p0 == 0xE7FFDEFF && p1 == 0xE7FFDEFF)
                {
                    p0 = MAGIC30;
                    p1 = MAGIC34;
                }

                Encrypt(ref p1, ref p0);
                Init1();
                Encrypt(ref p1, ref p0);

                writer.Write(p0);
                writer.Write(p1);
            }
        }

        /// <summary>
        /// First common initialization step
        /// </summary>
        private void Init1()
        {
            Buffer.BlockCopy(Constants.NDSEncryptionData, 0, cardHash, 0, 4 * (1024 + 18));
            arg2 = new uint[] { Gamecode, Gamecode >> 1, Gamecode << 1 };
            Init2();
            Init2();
        }

        /// <summary>
        /// Second common initialization step
        /// </summary>
        private void Init2()
        {
            Encrypt(ref arg2[2], ref arg2[1]);
            Encrypt(ref arg2[1], ref arg2[0]);

            byte[] allBytes = BitConverter.GetBytes(arg2[0])
                .Concat(BitConverter.GetBytes(arg2[1]))
                .Concat(BitConverter.GetBytes(arg2[2]))
                .ToArray();

            UpdateHashtable(allBytes);
        }

        /// <summary>
        /// Perform a decryption step
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Decrypt(ref uint arg1, ref uint arg2)
        {
            uint a = arg1;
            uint b = arg2;
            for (int i = 17; i > 1; i--)
            {
                uint c = cardHash[i] ^ a;
                a = b ^ Lookup(c);
                b = c;
            }

            arg1 = b ^ cardHash[0];
            arg2 = a ^ cardHash[1];
        }

        /// <summary>
        /// Perform an encryption step
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Encrypt(ref uint arg1, ref uint arg2)
        {
            uint a = arg1;
            uint b = arg2;
            for (int i = 0; i < 16; i++)
            {
                uint c = cardHash[i] ^ a;
                a = b ^ Lookup(c);
                b = c;
            }

            arg2 = a ^ cardHash[16];
            arg1 = b ^ cardHash[17];
        }

        /// <summary>
        /// Lookup the value from the hashtable
        /// </summary>
        /// <param name="v">Value to lookup in the hashtable</param>
        /// <returns>Processed value through the hashtable</returns>
        private uint Lookup(uint v)
        {
            uint a = (v >> 24) & 0xFF;
            uint b = (v >> 16) & 0xFF;
            uint c = (v >> 8) & 0xFF;
            uint d = (v >> 0) & 0xFF;

            a = cardHash[a + 18 + 0];
            b = cardHash[b + 18 + 256];
            c = cardHash[c + 18 + 512];
            d = cardHash[d + 18 + 768];

            return d + (c ^ (b + a));
        }

        /// <summary>
        /// Update the hashtable
        /// </summary>
        /// <param name="arg1"></param>
        private void UpdateHashtable(byte[] arg1)
        {
            for (int j = 0; j < 18; j++)
            {
                uint r3 = 0;
                for (int i = 0; i < 4; i++)
                {
                    r3 <<= 8;
                    r3 |= arg1[(j * 4 + i) & 7];
                }

                cardHash[j] ^= r3;
            }

            uint tmp1 = 0;
            uint tmp2 = 0;
            for (int i = 0; i < 18; i += 2)
            {
                Encrypt(ref tmp1, ref tmp2);
                cardHash[i + 0] = tmp1;
                cardHash[i + 1] = tmp2;
            }
            for (int i = 0; i < 0x400; i += 2)
            {
                Encrypt(ref tmp1, ref tmp2);
                cardHash[i + 18 + 0] = tmp1;
                cardHash[i + 18 + 1] = tmp2;
            }
        }
    }
}

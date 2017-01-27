using System;
using System.Collections.Generic;
using System.Text;
using Romulus.Plugin;
using System.IO;

namespace snarfblasm
{
    class AddressLabels : IAddressLabels
    {
        BankLabels ramLabels = new BankLabels(-1);
        BankLabelList bankLabels = new BankLabelList();

        public IBankLabels Ram {
            get { return ramLabels; }
        }

        public IBankLabelList Banks {
            get { return bankLabels; }
        }
    }

    class BankLabelList : IBankLabelList 
    {
        List<BankLabels> banks = new List<BankLabels>();
        const int bankIndexLimit = 255;

        internal IList<BankLabels> GetBanks() {
            return banks;
        }

        public IBankLabels this[int bankIndex] {
            get {
                if (bankIndex < 0 || bankIndex > bankIndexLimit)
                    throw new ArgumentException("Bank index is out of range.");

                for (int iBank = 0; iBank < banks.Count; iBank++) {
                    if (banks[iBank].BankIndex == bankIndex) {
                        return banks[iBank];
                    }
                }

                BankLabels newBank = new BankLabels(bankIndex);
                // Add the new bank object (in order)
                for (int iBank = 0; iBank < banks.Count; iBank++) {
                    // Insert before first bank that has higher index
                    if (banks[iBank].BankIndex > bankIndex) {
                        banks.Insert(iBank, newBank);
                        return newBank;
                    }
                }
                // If there is no bank with higher index, add the new bank to the end
                banks.Add(newBank);
                return newBank;
            }
        }
    }


    class BankLabels : IBankLabels
    {
        internal struct addressData
        {
            public string label;
            public string comment;
            public int size;
        }
        internal Dictionary<ushort, addressData> Labels = new Dictionary<ushort, addressData>();

        internal Dictionary<ushort, addressData> GetLabels() {
            return Labels;
        }


        public BankLabels(int bankIndex) {
            BankIndex = bankIndex;
        }

        public int BankIndex { get; private set; }

        public void AddComment(ushort address, string comment) {
            AddLabelData(address, null, 0, comment);
        }

        public void AddLabel(ushort address, string label) {
            AddLabelData(address, label, 0, null);
        }

        public void AddLabel(ushort address, string label, string comment) {
            AddLabelData(address, label, 0, comment);
        }

        public void AddArrayLabel(ushort address, string label, int byteCount) {
            AddLabelData(address, label, byteCount, null);
        }

        public void AddArrayLabel(ushort address, string label, int byteCount, string comment) {
            AddLabelData(address, label, byteCount, comment);
        }

        private void AddLabelData(ushort address, string label, int byteCount, string comment) {
            addressData data;

            // Get data for specified address, if present, and remove it from the dictionary so we can re-add updated data.
            if (Labels.TryGetValue(address, out data)) {
                Labels.Remove(address);
            } else {
                data = default(addressData);
            }

            if (label != null)
                data.label = label;
            if (comment != null)
                data.comment = comment;
            if (byteCount > 0)
                data.size = byteCount;

            Labels.Add(address, data);
        }

        /// <summary>
        /// Creates .NL files for the FCEUX debugger
        /// </summary>
        /// <param name="bankLabels"></param>
        public byte[] BuildNlFile() {
            MemoryStream outputStream = new MemoryStream();
            StreamWriter output = new StreamWriter(outputStream);



            var labels = GetLabels();
            foreach (var entry in labels) {
                var entryData = entry.Value;

                string countString = (entryData.size > 0) ? ("/" + entryData.size.ToString("X")) : (string.Empty);
                string nlEntry = "$" + entry.Key.ToString("X4") + countString + "#" + entryData.label + "#" + entryData.comment;

                output.WriteLine(nlEntry);
            }

            output.Flush();
            return outputStream.ToArray();
        }
    }
}

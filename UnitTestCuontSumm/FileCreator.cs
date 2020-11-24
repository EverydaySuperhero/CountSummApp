using System.IO;

namespace UnitTestCuontSumm
{
    public class FileCreator
    {
        public int summ=0;
        public long CreateFile(string filename,int count)
        {
            byte[] bytes = new byte[count];
            for(int i=0;i<count;i++)
            {
                bytes[i] = (byte)i;
            }
            File.WriteAllBytes(filename, bytes);
            return CalculateAmount(bytes);
        }
        private long CalculateAmount(byte[] bytes)
        {
            long summ = 0;
            foreach (var @byte in bytes)
            {
                summ += @byte;
            }
            return summ;
        }
    }
}

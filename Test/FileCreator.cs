using System.IO;

namespace Test
{
    public class FileCreator
    {
        public int summ=0;
        public void CreateFile(string filename,int count)
        {
            byte[] bytes = new byte[count];
            for(int i=0;i<count;i++)
            {
                bytes[i] = (byte)i;
            }
            File.WriteAllBytes(filename, bytes);
            summ = CalculateAmount(bytes);
        }
        private int CalculateAmount(byte[] bytes)
        {
            int summ = 0;
            foreach (var @byte in bytes)
            {
                summ += @byte;
            }
            return summ;
        }
    }
}

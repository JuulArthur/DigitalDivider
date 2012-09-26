
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;




namespace DigitalDivider
{
    public class Preservervation
    {

		/** Implementasjonseksempel hvis du ikke gidder å tyde kode:
		  	
			
			Preservation pres = new preservation();				//Deklarerer ny lagringpakke
		 
		    List<String> iWantToSaveThis = new List<String>();	//Liste som skal lagres
		
			iWantToSaveThis.Add("Piratkopiering er best Hilsen Juuls mor");      
			
			pres.setData(iWantTosaveThis);						//Seter pres til å inneholde ønsket data (Funksjonen tar en List<String> som argument)
		  
			pres.saveData();									//pres blir lagret på telefonen
			
			pres.loadData();									//pres blir hentet fra telefonen
		  
			pres.getData()										// Returnerer data i form av en liste med strings
			
		**/


		private List<String> Data = new List<String>();

		public Preservervation()
        {
            CreateFile();
        }


        public List<String> getData()
        {
			if (this.Data.Count == 0) { return new List<String>(); }
            return this.Data;
        }


        public void setData(List<String> input)
        {
			if (input.Count == 0){return;}
            this.Data = input;
        }



		//Sjekker om det eksisterer en fil, his ikke så blir det lagd en tom
        public void CreateFile() 
        {
            var file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists("preserve.dat"))
            {
                return;
            }
            using (var stream = new IsolatedStorageFileStream("preserve.dat", FileMode.Create, file))
            {
                return;
            }
        
        }


        //Skrive til fil
        public void saveData(){
            if (checkFileExist())
            {
                using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream("preserve.dat", FileMode.Open, FileAccess.Write, file))
                using (var writer = new StreamWriter(stream))
                {
                    for (int i = 0; i < this.Data.Count; i++)
                    {

                        writer.WriteLine(this.Data[i]);
                    }

                    writer.Close();
                    stream.Close();

                }
            }
            else
            {
                CreateFile();
            }

        }


        //Les fra fil
        public void loadData()
        {
            this.Data = new List<String>();
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
			using (var stream = new IsolatedStorageFileStream("preserve.dat", FileMode.Open, FileAccess.Read, file))
            using (var reader = new StreamReader(stream))
            {
                
                while (!reader.EndOfStream) { this.Data.Add(reader.ReadLine()); }
                reader.Close();
                stream.Close();
                return;
            }
        }

        //Sjekker Fileksistens
        private Boolean checkFileExist()
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            return file.FileExists("preserve.dat");
        }
         
        
    }
}

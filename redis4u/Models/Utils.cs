using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace redis4u.Models
{
    public class Utils
    {
        public static List<Animal> GetAnimals(string[] files, IHostingEnvironment env)
        {
            List<Animal> dogs = new List<Animal>();
            string pict, name;
            for (int ndx = 0; ndx < files.Length; ndx++)
            {
                pict = files[ndx].Replace(env.WebRootPath, "");
                name = pict.Substring(pict.IndexOf("dogs") + 5);
                name = name.Replace("_", " ");
                name = name.Substring(0, name.IndexOf("."));

                Animal dog = new Animal()
                {
                    PictureUrl = pict.Replace(@"\", @"/"),
                    Name = name
                };

                dogs.Add(dog);
            }
            return dogs;
        }

        public static string[] GetFiles(IHostingEnvironment env)
        {
            var webRoot = env.WebRootPath;
            string dir = string.Empty;

            if (OperatingSystem.IsWindows())
                dir = webRoot + @"\images\dogs";
            else
                dir = webRoot + @"/images/dogs";

            string[] files = System.IO.Directory.GetFiles(dir);
            return files;
        }

        public static List<AnimalVote> GetVotes(List<Animal> dogs,
            IDistributedCache cache)
        {
            List<AnimalVote> votes = new List<AnimalVote>();
            foreach (var item in dogs)
            {
                var strVote = cache.GetString(item.Name);
                int intVote = 0;
                if (!string.IsNullOrEmpty(strVote))
                {
                    intVote = Convert.ToInt32(strVote);
                }

                AnimalVote vote = new AnimalVote()
                {
                    Name = item.Name,
                    Count = intVote,
                    PictureUrl = item.PictureUrl,
                };

                votes.Add(vote);
            }

            return votes;
        }
    }
}

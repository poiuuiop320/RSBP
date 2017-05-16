using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSBP.Common
{
    public class YoutubeCategoryHelper
    {
        private IList<YoutubeCategory> youtubeCategory = new List<YoutubeCategory>();
        public IEnumerable<YoutubeCategory> YoutubeCategories
        {
            get
            {
                return youtubeCategory;
            }
        }

        public YoutubeCategoryHelper()
        {
            youtubeCategory.Add(new YoutubeCategory { Id = 1, Name = "Film & Animation" });
            youtubeCategory.Add(new YoutubeCategory { Id = 2, Name = "Autos & Vehicles" });
            youtubeCategory.Add(new YoutubeCategory { Id = 10, Name = "Music" });
            youtubeCategory.Add(new YoutubeCategory { Id = 15, Name = "Pets & Animals" });
            youtubeCategory.Add(new YoutubeCategory { Id = 17, Name = "Sports" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 18, Name = "Short Movies" });
            youtubeCategory.Add(new YoutubeCategory { Id = 19, Name = "Travel & Events" });
            youtubeCategory.Add(new YoutubeCategory { Id = 20, Name = "Gaming" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 21, Name = "Videoblogging" });
            youtubeCategory.Add(new YoutubeCategory { Id = 22, Name = "People & Blogs" });
            youtubeCategory.Add(new YoutubeCategory { Id = 23, Name = "Comedy" });
            youtubeCategory.Add(new YoutubeCategory { Id = 24, Name = "Entertainment" });
            youtubeCategory.Add(new YoutubeCategory { Id = 25, Name = "News & Politics" });
            youtubeCategory.Add(new YoutubeCategory { Id = 26, Name = "Howto & Style" });
            youtubeCategory.Add(new YoutubeCategory { Id = 27, Name = "Education" });
            youtubeCategory.Add(new YoutubeCategory { Id = 28, Name = "Science & Technology" });
            youtubeCategory.Add(new YoutubeCategory { Id = 29, Name = "Nonprofits & Activism" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 30, Name = "Movies" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 31, Name = "Anime/Animation" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 32, Name = "Action/Adventure" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 33, Name = "Classics" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 34, Name = "Comedy" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 35, Name = "Documentary" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 36, Name = "Drama" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 37, Name = "Family" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 38, Name = "Foreign" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 39, Name = "Horror" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 40, Name = "Sci-Fi/Fantasy" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 41, Name = "Thriller" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 42, Name = "Shorts" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 43, Name = "Shows" });
            //youtubeCategory.Add(new YoutubeCategory { Id = 44, Name = "Trailers" });

        }

    }

    public class YoutubeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

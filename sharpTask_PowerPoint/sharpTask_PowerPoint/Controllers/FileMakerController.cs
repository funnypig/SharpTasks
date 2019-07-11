using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spire.Presentation;

namespace sharpTask_PowerPoint.Controllers
{
    [Route("api/[controller]")]
    public class FileMaker : Controller
    {
        private static string[] possibleFonts = new[]
        {
            "Arial",
            "Helvetica",
            "Times New Roman",
            "Times",
            "Courier New",
            "Verdana"
        };

        [HttpGet("[action]")]
        public IEnumerable<string> GetFonts()
        {
            return possibleFonts;
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetSizes()
        {
            return Enumerable.Range(8, 32).Select(i => i.ToString());
        }


        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormCollection fileCollection, string fontName, string fontSize)
        {
            var file = fileCollection.Files[0];
            
            var name = "input."+file.FileName.Split('.').Last();
            using (var stream = new FileStream(name, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string newFileName = MakeFile(name, fontName, fontSize);

            return File(new FileStream(newFileName, FileMode.Open),
                "application/vnd.openxmlformats-officedocument.presentationml.presentation", newFileName);
        }
        
        public string MakeFile(string fileName, string fontName, string _fontSize)
        {
            int fontSize = 8;
            try
            {
                fontSize = int.Parse(_fontSize);
            }catch{}

            if (fontName == null)
                fontName = "Arial";
            
            var presentation = new Presentation();
            presentation.LoadFromFile(fileName);

            
            for (int i = 0; i < presentation.Slides.Count; i++)
            {
                try
                {
                    for (int j = 0; j<presentation.Slides[i].Shapes.Count; j++)
                    {
                        var shape = presentation.Slides[i].Shapes[j];
                        if (shape is IAutoShape)
                        {
                            ((IAutoShape) shape).TextFrame.TextRange.LatinFont = new TextFont(fontName);
                            ((IAutoShape) shape).TextFrame.TextRange.FontHeight = fontSize;
                        }       
                    }
                }
                catch 
                {
                    
                }
            }

            var newFileName = "output." + fileName.Split('.').Last();
            
            presentation.SaveToFile(newFileName, FileFormat.Auto);

            return newFileName;
        }
    }
}
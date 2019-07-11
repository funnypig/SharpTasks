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
            
            // save uploaded file
            var name = "input."+file.FileName.Split('.').Last();
            using (var stream = new FileStream(name, FileMode.Create))
            {
                 await file.CopyToAsync(stream);
            }
            
            //  change pptx 
            string newFileName = MakeFile(name, fontName, fontSize);
            
            // save output.pptx
            
            var mStream = new MemoryStream();
            using (var fStream = new FileStream(newFileName, FileMode.Open))
            {
                await fStream.CopyToAsync(mStream);
            }

            mStream.Position = 0;
            return Ok(mStream.ToArray());
            return File(mStream, System.Net.Mime.MediaTypeNames.Text.Xml, "output.pptx");
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
            
            TextFont mFont = new TextFont(fontName);
            
            for (int i = 0; i < presentation.Slides.Count; i++)
            {
                try
                {
                    for (int j = 0; j<presentation.Slides[i].Shapes.Count; j++)
                    {
                        if (presentation.Slides[i].Shapes[j] is IAutoShape)
                        {
                            var shape = presentation.Slides[i].Shapes[j] as IAutoShape;
                            if (shape.TextFrame != null)
                            {
                                foreach (TextParagraph p in shape.TextFrame.Paragraphs)
                                {
                                    foreach (TextRange text in p.TextRanges)
                                    {
                                        text.LatinFont = mFont;
                                        text.FontHeight = fontSize;
                                    }
                                }
                            }
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
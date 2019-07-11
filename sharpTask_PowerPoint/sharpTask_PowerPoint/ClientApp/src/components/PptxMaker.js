import React, {Component} from 'react';
import axios from "axios";
import saveAs from 'file-saver';

export class PptxMaker extends Component{
    static displayName = PptxMaker.name;
    
    
    constructor(props) {
        super(props);
        this.state = {fonts: [], 
            loading : true, 
            sizes : [], 
            sizeLoad : true,
            selectedFont:'arial', 
            selectedSize:'8',
            file : null
        };

        fetch('api/FileMaker/GetFonts')
            .then(response => response.json())
            .then(data => this.setState({
                fonts: data, 
                loading : false, 
                sizeLoad : this.state.sizeLoad, 
                sizes : this.state.sizes,
                selectedFont : this.state.selectedFont,
                selectedSize : this.state.selectedSize,
                file : this.state.file
            }));
        
        fetch('api/FileMaker/GetSizes')
            .then(response => response.json())
            .then(data => this.setState({
                sizes : data, 
                sizeLoad : false, 
                fonts : this.state.fonts, 
                loading : this.state.loading,
                selectedFont : this.state.selectedFont,
                selectedSize : this.state.selectedSize,
                file : this.state.file
            }));
        
        this.changeFile = this.changeFile.bind(this);
        this.upload = this.upload.bind(this);   
        this.setPreview = this.setPreview.bind(this);
        this.convertFile = this.convertFile.bind(this);
    }
    
    fontSelectionRenderer(fonts){
        console.log(fonts);
        return (
            <p className="chooseStyle"> Choose font:   
            <select id="fontSelection" onClick={this.setPreview}>
                {
                    fonts.map(font => <option>{font}</option>)
                }
            </select>
            </p>
        );
    }
    
    sizeSelectionRenderer(sizes){
        return(
            <p className="chooseStyle"> Choose size:
            <select id="sizeSelection" onClick={this.setPreview}>
                {
                    sizes.map(size => <option>{size.toString()}</option>)
                }
            </select>
            </p>
        )
    }
    
    setPreview(event){
        let sizeIndex = document.getElementById('sizeSelection').selectedIndex;
        let fontIndex = document.getElementById('fontSelection').selectedIndex;
        
        this.state.selectedFont = this.state.fonts[fontIndex];
        this.state.selectedSize = this.state.sizes[sizeIndex];
        
        document.getElementById('preview').setAttribute('style', 
            'font-family:'+this.state.selectedFont+';font-size:'+this.state.selectedSize+'px');
        
        console.log(event);
    }
    
    changeFile(e){
        this.state.hidden = true;
        this.state.file = e.target.files[0];
    }
    
    upload(e) {        
        e.preventDefault();
        
        if (this.state.file == null){
            alert("Upload again!");
            return;
        }
        if (this.state.file.length===0) {
            alert("Upload again!");
            return;
        }
        
        // some thing to prevent empty file

        let form = new FormData();
        
        form.append('fileCollection', this.state.file);
        
        form.append('fontName',this.state.selectedFont);
        form.append('fontSize',this.state.selectedSize);
        
        axios.post('api/FileMaker/Upload', form)
            .then(function(resp){
                console.log(resp);
                var blob = new Blob([resp.data], {type:"application/application/vnd.openxmlformats-officedocument.presentationml.presentation"});
                saveAs(blob, "output.pptx");
                
            })
            .catch(function (ex) {
                console.error(ex);
                alert("Error while uploading! Try again");
            });
    }
    
    convertFile(){
        axios.get("api/FileMaker/MakeFile", this.state.fileName, this.state.selectedFont, this.state.selectedSize);
    }
    
    render() {
        let fontSelection = this.state.loading
            ? <p><em>Loading...</em></p>
            :   this.fontSelectionRenderer(this.state.fonts);

        let sizeSelection = this.state.sizeLoad
            ? <p><em>Loading...</em></p>
            :   this.sizeSelectionRenderer(this.state.sizes);
            
            
        return (

            <body>
            <h1>Change font and size of your presentation!</h1>
            
                {fontSelection}
                {sizeSelection}


            <hr/>
                <div id='preview'>
                    This is preview text!
                </div>
            <hr/>
            
            <div>(initile text view is random, choose option)</div>

            <hr/>
                
                <div>
                    <input onClick={this.changeFile} type="file"/>
                    <button onClick={this.upload}>
                        Upload file
                    </button>
                </div>
            
            </body>
            );
    }
}
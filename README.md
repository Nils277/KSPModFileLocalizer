# KSPModFileLocalizer
This mod intends to automate the localization of static files for KSP like textures and KSPedia pages.

# How to use


### KSPedia entries
For each KSPedia file you want to translate you have to set up a config file with the following content:

    KSPediaLocalization
    {
        path = GameData/Path/to/KSPedia/
        filename = filename
        default = en-us
    }
The <b>path</b> describes the path to your KSPedia files. E.g. GameData/PlanetaryBaseInc/KSPedia/   
The <b>filename</b> is the name of your kspedia file. E.g. planetarybaseinc  
The <b>default</b> sets the which language is set by default when the language of the user is not supported  

All three fields are required.

For the files to work, you have to name them according to that scheme:

<b>filename</b> + "_" + language_tag + ".lang" for the files that can be translated and  
<b>filename</b> + "_" + language_tag + ".ksp" for the file that should be used.  

in the case of KPBS this would look like this:

    planetarybaseinc_es-es.lang
    planetarybaseinc_en-us.ksp
    localization.cfg


### Other static files
BEWARE, this feature is still in development and not fully tested yet! Feedback is welcome.  
For each static file you want to translate you have to set up a config file with the following content:

    FileLocalization
    {
        path = GameData/Path/to/file/
        filename = filename.extension
        default = en-us
    }
The <b>path</b> describes the path to your KSPedia files. E.g. GameData/PlanetaryBaseInc/Parts/Utility/  
The <b>filename</b> is the name of your kspedia file. E.g. cargobay.dds  
The <b>default</b> sets the which language is set by default when the language of the user is not supported  

For the files to work, you have to name them according to that scheme:

    

<b>filename</b> + "." + language_tag + ".lang" for the files that can be translated and  
<b>filename</b> for the file that should be used.  

in the case of KPBS the filder would something look like this:  
    
    cargobay.dds.es-es.lang 
    cargobay.dds  
    cargobay_loc.cfg  


# License

[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.html)

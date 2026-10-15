using System.Text;

namespace DigiWard.Voice.API.Services;


public class KnowledgeService
{

    private readonly IWebHostEnvironment _environment;


    public KnowledgeService(
        IWebHostEnvironment environment
    )
    {
        _environment = environment;
    }





    // =====================================
    // LOAD TOÀN BỘ KNOWLEDGE
    // =====================================

    public string LoadKnowledge()
    {

        var path =
            Path.Combine(
                _environment.ContentRootPath,
                "Knowledge"
            );


        if(!Directory.Exists(path))
        {
            return "";
        }



        var files =
            Directory.GetFiles(
                path,
                "*.md",
                SearchOption.AllDirectories
            );



        var builder =
            new StringBuilder();



        foreach(var file in files)
        {

            builder.AppendLine(
                File.ReadAllText(file)
            );


            builder.AppendLine(
                "\n-----------------\n"
            );

        }



        return builder.ToString();

    }






    // =====================================
    // SEARCH KNOWLEDGE (RAG MINI)
    // =====================================


    public string SearchKnowledge(
        string question
    )
    {

        var path =
            Path.Combine(
                _environment.ContentRootPath,
                "Knowledge"
            );



        if(!Directory.Exists(path))
        {
            return "";
        }




        var files =
            Directory.GetFiles(
                path,
                "*.md",
                SearchOption.AllDirectories
            );




        var keywords =
            question
            .ToLower()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );





        foreach(var file in files)
        {

            var content =
                File.ReadAllText(file);



            var lower =
                content.ToLower();




            foreach(var keyword in keywords)
            {

                if(lower.Contains(keyword))
                {
                    return content;
                }

            }

        }



        return "";

    }


}
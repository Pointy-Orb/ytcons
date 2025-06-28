namespace YTCons.UserExp;

public static class ExplainStuff
{
    //Write a file to the local directory that tells curious eyes what these variable are for.
    public static readonly string[] LocalFiles = new string[]
    {
        "These are the files saved to your computer by the ytcons program.",
        "You're probably wondering what they do. Well here's an explanantion:\n",
        "bestSite - When searching through the availible piped instances, there is a good chance that most of them don't work. However, there is usually one that does. That one is saved so that it can be referenced first before all the other instances. This improves laod times.\n",
        "instances.json - An offline instance list written after obtaining the online one. This is referenced if it is availible, and if all of its instances fail, then the online list is referred to again.\n"
    };
}

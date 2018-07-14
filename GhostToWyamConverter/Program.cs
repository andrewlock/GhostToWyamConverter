using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GhostToWyamConverter
{
    class Program
    {
        static (string from, string to)[] _pathsToReplace = new[]
        {
            ("/content/images/2017/04/dialog.png", "/content/images/dupes/01/dialog.png"),
            ("/content/images/2017/05/banner.png", "/content/images/dupes/02/banner.png"),
            ("/content/images/2017/05/banner.PNG", "/content/images/dupes/03/banner.PNG"),
            ("/content/images/2017/05/Compare.png", "/content/images/dupes/04/Compare.png"),
            ("/content/images/2017/10/banner.PNG", "/content/images/dupes/05/banner.PNG"),
            ("/content/images/2017/12/banner.png", "/content/images/dupes/06/banner.png"),
            ("/content/images/2018/01/banner-1.png", "/content/images/dupes/07/banner-1.png"),
            ("/content/images/2018/01/banner.png", "/content/images/dupes/08/banner.png"),
            ("/content/images/2018/02/banner.png", "/content/images/dupes/09/banner.png"),
            ("#thecakebuildscript", "#the-cake-build-script"),
            ("#thedockerfilecallingcakeinsidedocker","#the-dockerfile-calling-cake-inside-docker"),
            ("#thetwoversionsofcake","#the-two-versions-of-cake"),
            ("#addingcorrelationidsusingscope","#adding-correlation-ids-using-scope"),
            ("#replacingthedefaultconventionswithsnakecase","#replacing-the-default-conventions-with-snake-case"),
            ("#creatingloggingdelegateswiththeloggermessagehelper","#creating-logging-delegates-with-the-loggermessage-helper"),
            ("#thecookieauthenticationmiddleware","#the-cookie-authentication-middleware"),
            ("#referencemicrosoftaspnetcoreappinyourapps","#reference-microsoft-aspnetcore-app-in-your-apps"),
            ("#frameworkversionmismatches","#framework-version-mismatches"),
            ("#exactdependencyranges","#exact-dependency-ranges"),
            ("#1microsoftdotnet203runtimedeps","#1-microsoft-dotnet-2-0-3-runtime-deps"),
            ("#2microsoftdotnet203runtime","#2-microsoft-dotnet-2-0-3-runtime"),
            ("#3microsoftaspnetcore203runtime","#3-microsoft-aspnetcore-2-0-3"),
            ("#4microsoftdotnet203sdk","#4-microsoft-dotnet-2-0-3-sdk"),
            ("#5microsoftaspnetcorebuild203","#5-microsoft-aspnetcore-build-2-0-3"),
            ("#6microsoftaspnetcorebuild1020","#6-microsoft-aspnetcore-build-1-0-2-0"),
            ("#dontrequiresudotoexecutedockercommands","#don-t-require-sudo-to-execute-docker-commands"),
            ("#examiningthefilesystemofafaileddockerbuild","#examining-the-file-system-of-a-failed-docker-build"),
            ("#examiningthefilesystemofanimagewithanentrypoint","#examining-the-file-system-of-an-image-with-an-entrypoint"),
            ("#copyingfilesfromadockercontainertothehost","#copying-files-from-a-docker-container-to-the-host"),
            ("#copyingfilesfromadockerimagetothehost","#copying-files-from-a-docker-image-to-the-host"),
            ("#viewthespaceusedbydocker","#view-the-space-used-by-docker"),
            ("#removeolddockerimagesandcontainers","#remove-old-docker-images-and-containers-"),
            ("#speedingupbuildsbyminimisingthedockercontext","#speeding-up-builds-by-minimising-the-docker-context"),
            ("#viewingandminimisingthedockercontext","#viewing-and-minimising-the-docker-context"),
            ("#bonusmakingafileexecutableingit","#bonus-making-a-file-executable-in-git"),
            ("#bonus2forcingscriptfilestokeepunixlineendingsingit","#bonus-2-forcing-script-files-to-keep-unix-line-endings-in-git"),
            ("#3installrancherserver","#3-install-rancher-server"),
            ("#changingthedefaulttokenlifetime","#changing-the-default-token-lifetime"),
            ("#usingadifferentprovider","#using-a-different-provider"),
            ("#creatingadataprotectionbasedtokenproviderwithadifferenttokenlifetime","#creating-a-data-protection-based-token-provider-with-a-different-token-lifetime"),
            ("#usingonbuildtocreatebuilderimages", "#using-onbuild-to-create-builder-images"),
            ("#settingtheversionnumberwhenbuildingyourapplication", "#setting-the-version-number-when-building-your-application"),
            ("#abuilderimagethatsupportssettingtheversionnumber", "#a-builder-image-that-supports-setting-the-version-number"),
            ("#addingacustomnameforanonymousmiddleware", "#adding-a-custom-name-for-anonymous-middleware"),
            ("#creatinganiactionfilterthatreadsactionmethodparameters", "#creating-an-iactionfilter-that-reads-action-method-parameters"),
            ("#howtosettheversionnumberwhenyoubuildyourapplibrary", "#how-to-set-the-version-number-when-you-build-your-app-library"),
            ("#whathappensifbindingfails", "#what-happens-if-binding-fails-"),
            ("#1microsoftdotnet210runtimedeps", "#1-microsoft-dotnet-2-1-0-runtime-deps"),
            ("#2microsoftdotnet210runtime", "#2-microsoft-dotnet-2-1-0-runtime"),
            ("#3microsoftaspnetcore210aspnetcoreruntime", "#3-microsoft-dotnet-2-1-0-aspnetcore-runtime"),
            ("#4microsoftdotnet21300sdk", "#4-microsoft-dotnet-2-1-300-sdk"),
            ("#5microsoftaspnetcorebuild203", "#5-microsoft-aspnetcore-build-2-0-3"),
            ("#6microsoftaspnetcorebuild1020", "#6-microsoft-aspnetcore-build-1-0-2-0"),
            ("#settinghostingenvironmentusingcommandargs", "#setting-hosting-environment-using-command-args"),
            ("#thedefaultsettings", "#the-default-settings"),
            ("#writingacustomvalidatorforaspnetcoreidentity", "#writing-a-custom-validator-for-asp-net-core-identity"),
            ("#usingadedicateddataclasswithclassdata", "#using-a-dedicated-data-class-with-classdata-"),
            ("#creatinganewprincipal", "#creating-a-new-principal"),
            ("#choosingansdkversionwithglobaljson", "#choosing-an-sdk-version-with-global-json"),
            ("#multipleidentities", "#multiple-identities"),
            ("#howtolocalisedisplayattribute", "#how-to-localise-displayattribute"),
            ("#removingthemagicstrings", "#removing-the-magic-strings"),
            ("#applyingaglobalauthorisationrequirement", "#applying-a-global-authorisation-requirement"),
            ("#abriefintroductiontodockerfilesandlayercaching", "#a-brief-introduction-to-docker-files-and-layer-caching"),
            ("#theidealsolution", "#the-ideal-solution"),
            ("#thesolutiontarballupthecsproj", "#the-solution-tarball-up-the-csproj"),
            ("#resizingtheimage", "#resizing-the-image"),
            ("#protectingagainstdosattacks", "#protecting-against-dos-attacks"),
            ("https://andrewlock.net/", "/"),
            ("/content/images/2018/02/3.-Init-repo.PNG 2397w, 3. Init repo_2.PNG 1198w","/content/images/2018/02/3.-Init-repo.PNG 2397w, /content/images/2018/02/3.-Init-repo_2.PNG 1198w"),
            ("in the [previous post](/exploring-the-cookieauthenticationmiddleware-in-asp-net-core#theauthenticationhandlermiddleware)", "In the [previous post](/exploring-the-cookieauthenticationmiddleware-in-asp-net-core/)"),
            ("The [Serilog](github.com/serilog/serilog-aspnetcore)", "The [Serilog](https://github.com/serilog/serilog-aspnetcore)"),
            ("(loading-tenants-from-the-database-with-saaskit-part-2-caching)","(/loading-tenants-from-the-database-with-saaskit-part-2-caching/)"),
            ("(/home-home-on-the-range-installing-kubernetes-using-rancher-2-0/#2installdocker)","(/home-home-on-the-range-installing-kubernetes-using-rancher-2-0/#2-install-docker)"),
            ("(github.com/serilog/serilog-aspnetcore)", "(https://github.com/serilog/serilog-aspnetcore)"),
            ("(/using-a-culture-constraint-and-catching-404s-using-url-localisation-with-middleware-as-filters/)", "(/using-a-culture-constraint-and-catching-404s-with-the-url-culture-provider/)"),
            ("[couple of issues](/troubleshooting-asp-net-core-1-1-0-install-problems))", "[couple of issues](/troubleshooting-asp-net-core-1-1-0-install-problems/))"),
            ("[previous post](/exploring-middleware-as-mvc-filters-in-asp-net-core-1-1-preview-1)","[previous post](/exploring-middleware-as-mvc-filters-in-asp-net-core-1-1/)"),
            ("[previous post](an-introduction-to-viewcomponents-a-login-status-view-component)", "[previous post](/an-introduction-to-viewcomponents-a-login-status-view-component/)"),
            ("(#examining-the-file-system-of-an-image-with-an-entrypoint)","(/handy-docker-commands-for-local-development-part-1/#examining-the-file-system-of-an-image-with-an-entrypoint)"),
        };
        static void Main(string[] args)
        {
            // Example
            // args[0] = @"C:\input\andrew-lock-net-escapades.ghost.json";
            // args[1] = @"C:\output\posts";
            // args[2] = @"C:\output\drafts";

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Input filename and output location parameters are required");
                return;
            }

            Directory.CreateDirectory(args[1]);
            Directory.CreateDirectory(args[2]);
            foreach (var item in Directory.GetFiles(args[1]).Concat(Directory.GetFiles(args[2])))
            {
                File.Delete(item);
            }

            using (var stream = File.OpenRead(args[0]))
            using (var txt = new StreamReader(stream))
            using (var reader = new JsonTextReader(txt))
            {
                var jObject = JObject.Load(reader);

                // build tag list.
                var tags = jObject["db"].First["data"]["tags"].ToDictionary(tag => tag.Value<int>("id"), tag => tag.Value<string>("name"));

                var postTags = new Dictionary<int, List<int>>();
                foreach (var token in jObject["db"].First["data"]["posts_tags"])
                {
                    var postId = token.Value<int>("post_id");
                    var tagId = token.Value<int>("tag_id");
                    if (!postTags.ContainsKey(postId))
                    {
                        postTags.Add(postId, new List<int>());
                    }

                    postTags[postId].Add(tagId);
                }


                foreach (JToken post in jObject["db"].First["data"]["posts"])
                {
                    if(post.Value<int>("page") == 1)
                    {
                        //don't keep recreating these
                        continue;
                    }
                    string slug = post.Value<string>("slug");
                    var published = post.Value<string>("published_at");

                    bool isPublished = !string.IsNullOrEmpty(published) && post.Value<string>("status") != "draft";
                    var filePath = isPublished ? args[1] : args[2];

                    var filename = Path.Combine(filePath, slug + ".md");

                    Console.WriteLine("Creating " + filename);

                    using (var mdFile = File.CreateText(filename))
                    {
                        var postId = post.Value<int>("id");
                        mdFile.WriteLine("---");
                        mdFile.Write("title: ");
                        mdFile.WriteLine(Escape(post.Value<string>("title")));

                        if (isPublished)
                        {
                            var pubDate = DateTimeOffset.Parse(published);
                            mdFile.WriteLine("published_at: {0:O}", pubDate.ToString("dd MMM yyyy HH:mm:ss"));
                        }

                        var updated = post.Value<string>("updated_at");
                        if (isPublished)
                        {
                            var pubDate = DateTimeOffset.Parse(published);
                            mdFile.WriteLine("updated_at: {0:O}", pubDate.ToString("dd MMM yyyy HH:mm:ss"));
                        }

                        mdFile.Write("image: ");
                        string blogImage = post.Value<string>("image");
                        foreach (var pathpair in _pathsToReplace)
                        {
                            blogImage = blogImage?.Replace(pathpair.from, pathpair.to);
                        }
                        mdFile.WriteLine(blogImage);

                        if (postTags.ContainsKey(postId) && postTags[postId].Any())
                        {
                            mdFile.Write("tags: ");
                            mdFile.WriteLine(string.Join(", ", postTags[postId].Select(x => tags[x])));
                        }
                        string excerpt = Escape(post.Value<string>("meta_description"));
                        if (!string.IsNullOrEmpty(excerpt))
                        {
                            mdFile.Write("excerpt: ");
                            mdFile.WriteLine(excerpt);
                        }

                        mdFile.WriteLine("---");
                        string markdown = post.Value<string>("markdown");
                        var content = new StringBuilder(markdown);
                        content.Replace(@"```language-", @"```");
                        content.Replace(@"```language-", @"```");
                        content.Replace(@"``` language-", @"```");
                        foreach (var pathpair in _pathsToReplace)
                        {
                            content.Replace(pathpair.from, pathpair.to);
                        }
                        mdFile.Write(content.ToString());
                    }
                }
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }

        static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            if (value.Contains(":"))
            {
                return $@"""{value}""";
            }
            return value;
        }
    }
}

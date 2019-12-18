open System
open System.IO

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let download () =
    let paketPath =
        Directory.CreateDirectory(Path.Combine ([| __SOURCE_DIRECTORY__; ".paket" |])).FullName
    let paketFile = Path.Combine ([| paketPath; "paket.exe" |])
    let print msg = lock paketFile (fun () -> printfn "%s" msg)
    async {
        // if File.Exists (paketFile) then File.Delete (paketFile)
        if File.Exists (paketFile) |> not 
        then
            print "Downloading paket.exe"
            let url = "http://fsprojects.github.io/Paket/stable"
            use wc = new Net.WebClient ()
            let tmp = Path.GetTempFileName ()
            let! stable = wc.AsyncDownloadString (Uri url)
            wc.DownloadProgressChanged.Add(fun args ->
                args.ProgressPercentage.ToString("000") |> sprintf "Download %s%% complete" |> print)
            do! wc.AsyncDownloadFile (Uri stable, tmp)
            File.Move (tmp, paketFile)
            File.Delete (tmp)
            print "Finished downloading paket.exe"
    }
download () |> Async.RunSynchronously

//Newtonsoft.Json
let paketDependencies = """
source https://www.nuget.org/api/v2
nuget Newtonsoft.Json"""

System.IO.File.WriteAllText("paket.dependencies", paketDependencies)

let install () =
    let startInfo = 
        System.Diagnostics.ProcessStartInfo(
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            FileName = "./.paket/paket.exe",
            Arguments = "install"
        )
    use paket = new System.Diagnostics.Process(StartInfo = startInfo)
    use outputSub = paket.OutputDataReceived.Subscribe (fun arg -> printfn "%s" arg.Data)
    use errorSub = paket.ErrorDataReceived.Subscribe (fun arg -> eprintfn "%s" arg.Data)
    paket.Start () |> ignore
    paket.BeginOutputReadLine ()
    paket.BeginErrorReadLine ()
    paket.WaitForExit ()

install ()

#r "./packages/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"

open System

[<CLIMutable>]
type Test = {
    Id: Guid
    Name: string
    Date: DateTimeOffset
}

[<CLIMutable>]
type Meh = {
    Test: Test
    Whoa: int
}


let test = { Id = Guid.NewGuid(); Name = "meh"; Date = DateTimeOffset.Now }
let meh = { Test = test; Whoa = 100 }

let inline dump title (v) = printfn "%s: %A" title v; v

open System.Collections.Generic
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Linq

(*

class MyConverter : CustomCreationConverter<IDictionary<string, object>>
{
    public override IDictionary<string, object> Create(Type objectType)
    {
        return new Dictionary<string, object>();
    }

    public override bool CanConvert(Type objectType)
    {
        // in addition to handling IDictionary<string, object>
        // we want to handle the deserialization of dict value
        // which is of type object
        return objectType == typeof(object) || base.CanConvert(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject
            || reader.TokenType == JsonToken.Null)
            return base.ReadJson(reader, objectType, existingValue, serializer);

        // if the next token is not an object
        // then fall back on standard deserializer (strings, numbers etc.)
        return serializer.Deserialize(reader);
    }
}


*)

type DeepDictionaryConverter () =
    inherit CustomCreationConverter<IDictionary<string, obj>> ()

    override __.Create (objectType) =
        Dictionary<string, obj>() :> IDictionary<string, obj>

    override __.CanConvert (objectType) =
        objectType = typeof<obj> && base.CanConvert (objectType)

    override __.ReadJson (reader, objectType, existingValue, serializer) =
        let isStartObject = reader.TokenType = JsonToken.StartObject
        let isNull = reader.TokenType = JsonToken.Null
        if isStartObject || isNull
        then 
            base.ReadJson (reader, objectType, existingValue, serializer)
        else
            serializer.Deserialize (reader)

let deserializeObject =
    let converters = [| DeepDictionaryConverter () :> JsonConverter |]
    fun json -> JsonConvert.DeserializeObject<IDictionary<string, obj>> (json, converters)

let toDict (v: 'value) =
    let rec loop (v: JToken) =
        seq {
            for child in v.Children () do
                if child.HasValues
                then
                    yield! loop child
                else
                    yield child.Name, sprintf "%A" child.Value
        }
    JObject.FromObject (v) |> loop |> 

let runTest title = 
    JsonConvert.SerializeObject >> (dump (sprintf "%s RAW JSON" title)) >> deserializeObject >> (dump (sprintf "%s RESULT" title))


// dict ["This", "is a test."; "Meh", "Ryan 😎"; "Whoa", "Tyler 🤢"] |> runTest "Dictionary Test"
// test |> runTest "Simple Object Test"
meh |> runTest "Not Very Simple Object Test"
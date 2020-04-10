// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text

let hash length content = 
    let mutable initialValue = 312234567
    let mutable bytes = content
    for i in 1..25 do
        bytes <- Seq.scan (fun acc curr -> ( (acc * 27 + ( (int curr) ) ) ) ) initialValue bytes
        initialValue <- initialValue * 12 * (Seq.head bytes) + 82   

    let result = bytes
                 |> Seq.map (fun i -> Convert.ToString(i, 2))
                 |> Seq.concat

    let toSkip = ( Seq.length result ) - length

    result 
    |> Seq.skip toSkip
    |> Seq.toArray |> String

let step1 () =
    let contents = seq {"asdfvhj";"asdfghj";"0000000";"0000001";"0000002";"0000003";"0000004";"aaaaaab";"aaaaaac";"aaaaaad";"faafaaa"}
    
    for content in contents do
        let hash = hash 8 ((System.Text.Encoding.Unicode.GetBytes(content)) |> Seq.map int)
        printfn "%s\r\n" hash

let step2_3 () = 
    async  {
        let! png = File.ReadAllBytesAsync("fsharp256.png") |> Async.AwaitTask
        let! docx = File.ReadAllBytesAsync("Lb3.docx") |> Async.AwaitTask
        let! source = File.ReadAllBytesAsync("Program.fs") |> Async.AwaitTask
        
        let byteToInt = Seq.map (fun i -> int i)

        let pngHash = hash 8 (byteToInt png)
        let docxHash = hash 8 (byteToInt docx)
        let sourceHash = hash 8 (byteToInt source)

        printfn "pngHash: %s\r\ndocxHash:%s\r\nsourceHash:%s\r\n" pngHash docxHash sourceHash
        ()
    }       

let rnd = new Random()

let generateRandomString (len:int) =
    let mutable sbuilder = new StringBuilder(len)
    for _ in 1..len do
        sbuilder.Append(char (rnd.Next(48, 123))) |> ignore
    sbuilder.ToString ()

let checkCollisions () =
    let collisions = Seq.init 75 (fun _ -> generateRandomString (rnd.Next(5,20)) )
                     |> Seq.groupBy (fun i -> hash 8 (System.Text.Encoding.Unicode.GetBytes(i) |> Seq.map int)  )
                     |> Seq.where (fun (hash, elems) -> Seq.length elems > 1)
    for (hash, values) in collisions do
        printfn "%s hash collision with values: %A" hash values
    
[<EntryPoint>]
let main argv = 
    checkCollisions () // |> Async.RunSynchronously
    0 // return an integer exit code

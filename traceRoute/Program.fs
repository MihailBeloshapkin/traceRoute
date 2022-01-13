open System
open System.Net.NetworkInformation

let timeout = 10000
let maxTtl = 30
let sizeOfBuffer = 32

let getRouteInfo (ip : string) =
    let buffer : byte [] = Array.zeroCreate sizeOfBuffer
    let rnd = new Random()
    rnd.NextBytes(buffer)
    use ping = new Ping()
    let rec sub ttl data =
        if ttl = maxTtl then data else
            let options = new PingOptions(ttl, true)
            let reply = ping.Send(ip, timeout, buffer, options)
            match reply.Status with
            | IPStatus.Success -> reply.Address.ToString() :: data
            | IPStatus.TtlExpired -> sub (ttl + 1) (reply.Address.ToString() :: data)
            | IPStatus.TimedOut -> sub (ttl + 1) ("#########" :: data)
            | _ -> failwith "Something is wrong"
    sub 1 []

[<EntryPoint>]
let main argv =
    printfn "Input ip:"
    let host = Console.ReadLine()
    let result = host |> getRouteInfo |> List.rev 
    printfn "All trace:"
    for i in 0 .. result.Length - 1 do printf "%s\n" result.[i]
    0
﻿module Components

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Models

// ReactHelper defines a DSL to make it easier to build
// React components from F#
module R = Fable.Helpers.React
open R.Props
open Fable.Import.React

type MathBoxState = { data: string; showHints: bool }
type AnswerState = | NeedsReview | Good | NoAnswer
type HintState = (int * AnswerState ref) list list
type HintProps = { size: int }

type HintTable(props: HintProps) as this = 
    inherit React.Component<HintProps, HintState>()
    do this.state <- [1..props.size] |> List.map (fun x ->
                                            [1..props.size] |> List.map (fun y -> x * y, ref NoAnswer))
    member x.render() =
        let makeCell (cell: int * AnswerState ref) =
            let needsReview = !(snd cell)
            R.td [ClassName (needsReview |> function | NeedsReview -> "hintcell needsreview" | Good -> "hintcell correct" | NoAnswer -> "hintcell")] [unbox (fst cell)]

        let rows = this.state |> List.map (fun rowvals -> R.tr [] (rowvals |> List.map makeCell))
        R.table [ClassName "hinttable"] [R.tbody [] rows]

type MathBox() as this =
    inherit React.Component<unit, MathBoxState>()
    let prob = MathProblems()
    do this.state <- { data = prob.CurrentProblem; showHints = false }    
    member x.render () = 
        let numKey (n: int) =
            R.button [
                    OnClick (fun _ ->
                                prob.Advance()
                                x.setState { x.state with data = prob.CurrentProblem })
                    ClassName "numkey"
                ] [unbox (n.ToString())]
        // there's a shell which holds both the hint box and the interaction keypad
        R.div [ClassName "shell"] [
            R.div [ClassName "keypad"] [
                // Use ReactHelper.com to build a React Component from a type
                R.h2 [ClassName "numDisplay"] [unbox x.state.data]
                R.div [ClassName "keyList"] [
                    numKey 1
                    numKey 2
                    numKey 3
                    numKey 4
                    numKey 5
                    numKey 6
                    numKey 7
                    numKey 8
                    numKey 9
                    R.button [ClassName "numkey"] [unbox "Backspace"]
                    numKey 0
                    R.button [ClassName "numkey"] [unbox "ENTER"]
                    R.button [
                        ClassName "numkey"
                        OnClick (fun _ ->
                                  x.setState({ x.state with showHints = not x.state.showHints })
                        )
                    ] [unbox (if x.state.showHints then "Hide hints" else "Show hints")]
                ]
            ]
            R.div 
              [
                    ClassName (if x.state.showHints then "hintbox" else "hintbox hide")
              ] 
              [
                R.com<HintTable, HintProps, HintState> { size = 12 } []
              ]
        ]

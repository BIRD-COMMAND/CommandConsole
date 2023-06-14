using System.Collections;
using UnityEngine;

public abstract class ConsoleCommand
{

	public ConsoleCommand(CommandConsole console) { Console = console; }
	
	public CommandConsole Console { get; set; }
	
	public abstract string Trigger { get; }
	public abstract string HelpText { get; }
	public abstract string ArgHelpText { get; }

	public abstract void Execute(params string[] args);
	
	public Coroutine Coroutine { get; set; }
	public virtual IEnumerator CommandCoroutine() { return null; }
	public void StartCoroutine() { Coroutine = CoroutineUtil.Start(CommandCoroutine()); }
	public void StopCoroutine() { CoroutineUtil.Stop(Coroutine); Coroutine = null; }
	public void ToggleCoroutine()
	{
		if (Coroutine != null) { 
			StopCoroutine();  Console.AddLine($"{Trigger} coroutine stopped"); 
		} else { 
			StartCoroutine(); Console.AddLine($"{Trigger} coroutine started"); 
		}
	}

}
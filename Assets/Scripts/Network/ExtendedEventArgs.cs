using System;
using System.ComponentModel;

public class ExtendedEventArgs : EventArgs
{

	public int event_id { get; set; }

	public override string ToString()
	{
		string output = "";

		foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
		{
			string name = descriptor.Name;
			object value = descriptor.GetValue(this);
			output += name + " " + value + "\n";
		}
		return output;
	}
}
﻿using ConsoleMultiplexer.Controls;
using ConsoleMultiplexer.Data;
using ConsoleMultiplexer.Input;
using ConsoleMultiplexer.Space;
using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleMultiplexer.Example
{
	class TestContext : IDrawingContext
	{
		public Size MinSize => new Size(100, 30);
		public Size MaxSize => new Size(100, 30);

		//Character[,] _memory = new Character[100, 30];

		private IControl _control;
		public IControl Control
		{
			get => _control;
			set
			{
				_control = value;
				_control.Context = this;
				Redraw(_control);
			}
		}

		public void Redraw(IControl control)
		{
			if (control != _control) return;

			Update(control, control.Size.AsRect());
		}

		public void Update(IControl control, in Rect rect)
		{
			if (control != _control) return;

			foreach (var position in rect)
			{
				var c = control[Position.At(position.X, position.Y)];

				//if (c.Content != _memory[position.X, position.Y].Content)
				{
					//_memory[position.X, position.Y] = c;

					var content = c.Content ?? ' ';
					var foreground = c.Foreground ?? Color.White;
					var background = c.Background ?? Color.Black;

					Console.SetCursorPosition(position.X, position.Y);
					Console.Write($"\x1b[38;2;{foreground.Red};{foreground.Green};{foreground.Blue}m\x1b[48;2;{background.Red};{background.Green};{background.Blue}m{content}");
				}
			}

			Console.BackgroundColor = ConsoleColor.Black;
		}

		public event SizeLimitsChangedHandler SizeLimitsChanged;
	}

	class Program
	{
		static void Main(string[] args)
		{
			Console.SetWindowSize(1, 1);
			Console.SetBufferSize(200, 5);
			Console.SetWindowSize(200, 50);
			Console.CursorVisible = false;

			var testContext = new TestContext();

			var textBlock1 = new TextBlock
			{
				Text = "Heheszki1",
				Color = new Color(87, 200, 157)
			};

			var border1 = new Border
			{
				BorderPlacement = BorderPlacement.All ^ BorderPlacement.Bottom,
				Content = textBlock1
			};

			var textBlock2 = new TextBlock
			{
				Text = "Heheszki2",
				Color = new Color(157, 42, 157)
			};

			var border2 = new Border
			{
				BorderPlacement = BorderPlacement.Left | BorderPlacement.Right,
				Content = new HorizontalAlignment
				{
					Content = textBlock2
				},
				BorderColor = new Color(200, 0, 0)
			};

			var textBox = new TextBox();

			var stackPanel1 = new VerticalStackPanel();
			stackPanel1.Add(new TextBlock { Text = "Test1" });
			stackPanel1.Add(new TextBlock { Text = "Test2" });
			stackPanel1.Add(border1);
			stackPanel1.Add(border2);
			stackPanel1.Add(new WrapPanel
			{
				Children = new IControl[] {
					new TextBlock
					{
						Color = Color.LightBlue,
						Text = @"D:\test> "
					},
					textBox
				}
			});

			var border3 = new Border
			{
				BorderPlacement = BorderPlacement.All,
				Content = stackPanel1,
				BorderColor = new Color(100, 100, 50)
			};

			//var textBlock2 = new TextBlock
			//{
			//	Text = "Heheszki2"
			//};

			//var stackPanel = new VerticalStackPanel();
			//stackPanel.Add(new Border { Content = new TextBlock { Text = "Counter" } });
			//stackPanel.Add(textBlock1);
			//stackPanel.Add(new Border { Content = new TextBlock { Text = "Stoper" } });
			//stackPanel.Add(textBlock2);

			//var border = new Border()
			//{
			//	BorderPlacement = BorderPlacement.Left | BorderPlacement.Top | BorderPlacement.Right,
			//	Content = stackPanel
			//};

			var border4 = new Border
			{
				Content = new Background
				{
					Fill = new Character('.', new Color(123, 54, 34), new Color(65, 25, 235)),
					Content = new HorizontalAlignment
					{
						Content = new Style
						{
							Foreground = new Color(64, 132, 54),
							Background = new Color(65, 31, 64),
							Content = new TextBlock
							{
								Text = "Test"
							}
						}
					}
				}
			};

			var scrollPanel = new VerticalScrollPanel
			{
				Top = 0,
				Content = new VerticalStackPanel
				{
					Children = new[] {
							new TextBlock {Text = "Test 1"},
							new TextBlock {Text = "Test 2"},
							new TextBlock {Text = "Test 3"},
							new TextBlock {Text = "Test 4"},
							new TextBlock {Text = "Test 5"},
							new TextBlock {Text = "Test 6"},
							new TextBlock {Text = "Test 7"},
							new TextBlock {Text = "Test 8"},
							new TextBlock {Text = "Test 9"},
							new TextBlock {Text = "Test 10"},
							new TextBlock {Text = "Test 11"},
							new TextBlock {Text = "Test 12"},
							new TextBlock {Text = "Test 13"},
							new TextBlock {Text = "Test 14"},
							new TextBlock {Text = "Test 15"}
						}
				}
			};

			var border5 = new Border
			{
				Content = scrollPanel
			};

			var textBlock4 = new TextBlock
			{
				Text = "Heheszki2",
				Color = new Color(157, 42, 157)
			};

			var border6 = new Border
			{
				Content = new WrapPanel
				{
					Children = new[] {
						new TextBlock {Text = "Test 1"},
						new TextBlock {Text = "Test 2"},
						new TextBlock {Text = "Test 3"},
						textBlock4,
						new TextBlock {Text = "Test 4"},
						new TextBlock {Text = "Test 5"},
						new TextBlock {Text = "Test 6"},
						new TextBlock {Text = "Test 7"},
					}
				}
			};

			var canvas = new Canvas();
			canvas.Add(border3, new Rect(20, 10, 70, 20));
			canvas.Add(border4, new Rect(40, 5, 60, 10));
			canvas.Add(border5, new Rect(35, 16, 40, 10));
			canvas.Add(border6, new Rect(5, 5, 10, 15));

			testContext.Control = canvas;

			int frames = 0;
			var watch = new Stopwatch();
			watch.Start();

			for (int i = 0; ; i++)
			{
				textBlock1.Text = $"{i}";

				if (watch.ElapsedMilliseconds > 1000)
				{
					textBlock2.Text = $"{frames}";
					watch.Restart();
					frames = 0;
					border3.BorderPlacement ^= BorderPlacement.Left;
					textBlock4.Text = new Random().Next().ToString();
				}

				while (Console.KeyAvailable)
				{
					var key = Console.ReadKey(true);
					var inputEvent = new InputEvent(key);
					if (!inputEvent.Handled) (textBox as IInputListener).OnInput(inputEvent);
					if (!inputEvent.Handled) (scrollPanel as IInputListener).OnInput(inputEvent);
				}
				Console.CursorVisible = false;

				frames++;
			}
		}
	}
}

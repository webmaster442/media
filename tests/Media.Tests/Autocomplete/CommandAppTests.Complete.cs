using Media.Core.AutoComplete;
using Media.Tests.Autocomplete.Commands;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Testing;

namespace Media.Tests.Autocomplete;

public sealed partial class CommandAppTests
{
    [TestFixture]
    public sealed class Complete
    {
        private CommandAppTester CreateCommandApp(Action<IConfigurator> action)
        {
            var fixture = new CommandAppTester();
            fixture.Configure(config =>
            {
                config.AddAutoCompletion();
                config.SetApplicationName("myapp");
                config.PropagateExceptions();
                action(config);
            });
            return fixture;
        }

        [Test]
        public void Should_Return_Correct_Completions_When_There_Is_A_Partial_Command_Typed_In()
        {
            // Given
            var fixture = CreateCommandApp(config => config.AddCommand<LionCommand>("lion"));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp li\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("lion"));
        }

        [Test]
        public void Should_Return_Correct_Completions_When_There_Is_More_Than_One_Command()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddCommand<LionCommand>("lion");
                config.AddCommand<CatCommand>("cat");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("completion\nlion\ncat"));
        }

        [Test]
        public void Should_Return_Correct_Completions_When_There_Is_A_Branch()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("animal", animal =>
                {
                    animal.AddCommand<LionCommand>("lion");
                    animal.AddCommand<CatCommand>("cat");
                });
            });
            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("completion\nanimal"));
        }

        [Test]
        public void Should_Return_Correct_Completions_When_We_Are_In_A_Branch()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("animal", animal =>
                {
                    animal.AddCommand<LionCommand>("lion");
                    animal.AddCommand<CatCommand>("cat");
                });
            });
            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp animal \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("lion\ncat"));
        }

        [Test]
        public void Should_Return_Correct_Completions_When_There_Are_Multiple_Branches()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<LionCommand>("lion");
                    feline.AddCommand<CatCommand>("cat");
                });
                config.AddBranch("other", other =>
                {
                    other.AddCommand<DogCommand>("dog");
                    other.AddCommand<HorseCommand>("horse");
                });
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp other \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("dog\nhorse"));
        }

        [Test]
        public void Should_Return_Correct_Completions_When_There_Are_Many_Options_With_Same_Initial()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                // This one should not appear in the completions
                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp f\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("feline\nfantasy"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Parameters()
        {
            var fixture = CreateCommandApp(config => config.AddCommand<LionCommand>("lion"));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion 2 4 \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--alive\n--not-dead\n--name\n--pet-name\n--agility"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Parameters_When_Partial_Parameter_Name_Is_Provided()
        {
            var fixture = CreateCommandApp(config => config.AddCommand<LionCommand>("lion"));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion --n\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--not-dead\n--name"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Arguments_When_Partial_Argument_Value_Is_Provided1()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddCommand<CatCommand>("cat");
                config.AddCommand<LionCommand>("lion");
            });

            // Legs TEETH
            // Legs should be completed, because it does not have a trailing space
            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion 1\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("10"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Arguments_When_Trailing_Space()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddCommand<CatCommand>("cat");
                config.AddCommand<LionCommand>("lion");
            });

            // Legs TEETH // TEEH should be completed
            // Teeth should be completed, because it does have a trailing space
            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion 2 \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("32"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Parameters_Name_Should_Be_Angelika()
        {
            var fixture = CreateCommandApp(config => config.AddCommand<LionCommand>("lion"));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion 2 4 --name \"")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("Angelika"));
        }

        [Test]
        public void Should_Return_Correct_Completions_For_Parameters_When_AlreadyFullyWritten()
        {
            var fixture = CreateCommandApp(config => config.AddCommand<LionCommand>("lion"));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp lion 2 4 --name\"")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--name"));
        }

        [Test]
        public void Should_Return_Correct_Completions_In_Branch()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp feline l\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("lion"));
        }

        [Test]
        public void Should_Return_Nothing_When_Match_Exact_But_No_Trailing_Space()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp feline\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Should_Return_Completions_When_Match_Exact_And_Has_Trailing_Space()
        {

            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp feline \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("felix\nlion"));
        }

        [Test]
        public void Should_Return_All_Completions_When_No_Command()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("completion\nfeline\nfantasy\nlion"));
        }

        [Test]
        public void Should_Return_All_Completions_When_No_Command_But_Also_No_Trailing_Space()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("completion\nfeline\nfantasy\nlion"));
        }

        [Test]
        public void Should_Handle_Positions()
        {

            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp feline\"")
                   .Append("--position")
                   .Append("12")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Should_Handle_Positions1()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp feline\"")
                   .Append("--position")
                   .Append("13")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("felix\nlion"));
        }

        [Test]
        public void Should_Handle_Positions2()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("feline", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("fantasy", other => other.AddCommand<EmptyCommand>("fairy"));

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp\"")
                   .Append("--position")
                   .Append("6")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("completion\nfeline\nfantasy\nlion"));
        }

        // Test:
        // myapp [branch] [command] [dynamic_argument]
        // "myapp cats lion 1" <- should return 16
        [Test]
        public void Should_Handle_Positions3()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("cats", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddBranch("dogs", feline =>
                {
                    feline.AddCommand<CatCommand>("felix");
                    feline.AddCommand<LionCommand>("lion");
                });

                config.AddCommand<LionCommand>("lion");
            });

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp cats lion 1\"")
                   .Append("--position")
                   .Append("17")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("10"));
        }

        [Test]
        public void CompletionSuggestionsAttribute_Should_Suggest_Option_Values_Starting_With_Partial()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add angel --age 1\"")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("10\n15"));
        }

        [Test]
        public void CompletionSuggestionsAttribute_Should_Suggest_Option_Values()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add angel --age \"")
                   ;

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("10\n15\n20\n30"));
        }

        [Test]
        public void CompletionSuggestionsAttribute_Should_Suggest_Argument_Values()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("Angelika\nArnold\nBernd\nCloud\nJonas"));
        }

        [Test]
        public void CompletionSuggestionsAttribute_Should_Suggest_Argument_Values_With_Partial()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add a\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("Angelika\nArnold"));
        }

        // "myapp user add angel --age 1 --" should not suggest --age, only --gender
        // "myapp user add angel --age 1 --g" should not suggest --age, only --gender
        [Test]
        public void Suggestion_Should_Not_Contain_Options_Which_Are_already_present()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add angel --age 1 --\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--gender"));
        }

        [Test]
        public void Partial_Suggestion_Should_Not_Contain_Options_Which_Are_already_present()
        {
            var fixture = CreateCommandApp(config => config.AddBranch("user", feline => feline.AddCommand<UserAddCommand>("add")));

            var commandToRun = Constants.CompleteCommand
                   .Append("\"myapp user add angel --age 1 --g\"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--gender"));
        }

        [Test]
        public void Completion_Should_Not_Suggest_Anything_When_CommandArgument_Is_Dynamic_And_No_Handler_Registered()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("user", feline =>
                {
                    feline.AddCommand<UserAddCommand>("add");
                    feline.AddCommand<UserSuperAddCommand>("superAdd");
                });
            });

            // We expect a name to be entered.
            // Since no handler is registered for the command, we don't know what to suggest.
            // So we shouldn't suggest anything.
            var commandToRun = Constants.CompleteCommand
                .Append("\"myapp user superAdd \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(string.IsNullOrWhiteSpace(result.Output), Is.True, message: "Output should be empty. Actual: " + result.Output);
        }

        [Test]
        public void Completion_Should_Not_Suggest_Anything_When_CommandOption_Is_Dynamic_And_No_Handler_Registered()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("user", feline =>
                {
                    feline.AddCommand<UserAddCommand>("add");
                    feline.AddCommand<UserSuperAddCommand>("superAdd");
                });
            });

            // We expect a name to be entered.
            // Since no handler is registered for the command, we don't know what to suggest.
            // So we shouldn't suggest anything.
            var commandToRun = Constants.CompleteCommand
                .Append("\"myapp user superAdd Josh --gender \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(string.IsNullOrWhiteSpace(result.Output), Is.True, message: "Output should be empty. Actual: " + result.Output);
        }

        [Test]
        public void Completion_Should_Suggest_Remaining_Options()
        {
            var fixture = CreateCommandApp(config =>
            {
                config.AddBranch("user", feline =>
                {
                    feline.AddCommand<UserAddCommand>("add");
                    feline.AddCommand<UserSuperAddCommand>("superAdd");
                });
            });

            // We expect a name to be entered.
            // Since no handler is registered for the command, we don't know what to suggest.
            // So we shouldn't suggest anything.
            var commandToRun = Constants.CompleteCommand
                .Append("\"myapp user superAdd Josh --gender male \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--age"));
        }

        [Test]
        public void CustomTypeResolver_Works()
        {
            var services = new ServiceCollection();
            var customRegistry = new CustomTypeRegistrar(services);

            var fixture = new CommandAppTester(customRegistry);
            fixture.Configure(config =>
            {
                config.AddAutoCompletion();
                config.SetApplicationName("myapp");
                config.PropagateExceptions();

                config.AddBranch("user", feline =>
                {
                    feline.AddCommand<UserAddCommand>("add");
                    feline.AddCommand<UserSuperAddCommand>("superAdd");
                });
            });

            var commandToRun = Constants.CompleteCommand
              .Append("\"myapp user superAdd Josh --gender male \"");

            // When
            var result = fixture.Run(commandToRun.ToArray());

            // Then
            Assert.That(result.Output, Is.EqualTo("--age"));
        }
    }
}
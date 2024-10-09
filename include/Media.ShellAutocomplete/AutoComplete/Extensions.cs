// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using JKToolKit.Spectre.AutoCompletion.Completion.Internals;

using Spectre.Console.Cli;

namespace Media.ShellAutoComplete.AutoComplete;

public static class Extensions
{
    public static AsyncCommandParameterMatcher<TSettings> MatchAsync<TSettings>(this Command<TSettings> command)
        where TSettings : CommandSettings
    {
        return new AsyncCommandParameterMatcher<TSettings>();
    }

    public static IConfigurator AddAutoCompletion(this IConfigurator configurator, Action<IConfigurator<CommandSettings>>? action = null)
    {
        configurator.AddBranch("completion", complete =>
        {
            complete.AddCommand<CompleteCommand>("complete").IsHidden();

            if (action is not null)
            {
                action(complete);
            }
        });

        return configurator;
    }
}
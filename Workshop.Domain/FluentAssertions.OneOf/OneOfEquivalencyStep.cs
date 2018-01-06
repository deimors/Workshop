using FluentAssertions.Equivalency;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FluentAssertions.OneOf
{
	public class OneOfEquivalencyStep : IEquivalencyStep
	{
		private const string IsTPropertyNameRegex = @"^IsT(\d+)$";
		private const string AsTPropertyNamePrefix = "AsT";

		public bool CanHandle(IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
			=> IsOneOfType(context.RuntimeType);

		private static bool IsOneOfType(Type type)
			=> typeof(IOneOf).IsAssignableFrom(type);

		public bool Handle(IEquivalencyValidationContext context, IEquivalencyValidator parent, IEquivalencyAssertionOptions config)
		{
			EvaluateSubjectAndExpectationTypeEquality(context);

			var selectedMembers = GetSelectedMembers(context, config).ToArray();

			EvaluateNonAsMemberEquality(selectedMembers, context, config);

			EvaluateActiveAsMemberEquality(selectedMembers, context, config);

			return true;
		}

		private static void EvaluateSubjectAndExpectationTypeEquality(IEquivalencyValidationContext context)
			=> context.Subject.GetType().Should().Be(context.Expectation.GetType());

		private static void EvaluateNonAsMemberEquality(IEnumerable<SelectedMemberInfo> selectedMembers, IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
		{
			var nonAsMembers = selectedMembers.Where(info => !info.Name.StartsWith(AsTPropertyNamePrefix));

			foreach (var member in nonAsMembers)
				EvaluateEquality(member, context, config);
		}

		private static void EvaluateActiveAsMemberEquality(IEnumerable<SelectedMemberInfo> selectedMembers, IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
		{
			var activeIsMember = GetSingleActiveIsMember(selectedMembers, context);

			activeIsMember.Should().NotBeNull($"{context.Subject.GetType().Name} should have exactly one true IsT# property");

			var isNumber = GetIsMemberNumber(activeIsMember);

			var asMember = GetAsMemberByNumber(selectedMembers, isNumber);

			if (asMember != null)
				EvaluateEquality(asMember, context, config);
		}

		private static void EvaluateEquality(SelectedMemberInfo memberInfo, IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
		{
			var memberMatch = FindMatchFor(memberInfo, context, config);

			memberMatch.Should().NotBeNull($"{context.Expectation.GetType().Name} should have a matching member for {memberInfo.Name}");

			var subjectProperty = memberInfo.GetValue(context.Subject, new object[0]);

			var expectedProperty = memberMatch.GetValue(context.Expectation, new object[0]);

			subjectProperty.ShouldBeEquivalentTo(expectedProperty, options => options.Using(new ExcludeOneOfAsMembersSelectionRule()), context.Because, context.BecauseArgs);
		}

		private static SelectedMemberInfo FindMatchFor(SelectedMemberInfo selectedMemberInfo, IEquivalencyValidationContext context, IEquivalencyAssertionOptions config)
			=> config.MatchingRules
				.Select(rule => rule.Match(selectedMemberInfo, context.Expectation, context.SelectedMemberDescription, config))
				.FirstOrDefault(match => match != null);

		private static IEnumerable<SelectedMemberInfo> GetSelectedMembers(ISubjectInfo context, IEquivalencyAssertionOptions config)
			=> config.SelectionRules.Aggregate(
				Enumerable.Empty<SelectedMemberInfo>(),
				(members, selectionRule) => selectionRule.SelectMembers(members, context, config)
			);

		private static SelectedMemberInfo GetSingleActiveIsMember(IEnumerable<SelectedMemberInfo> selectedMembers, IEquivalencyValidationContext context)
			=> selectedMembers
				.Where(info => Regex.IsMatch(info.Name, IsTPropertyNameRegex))
				.SingleOrDefault(info => context.Subject.GetType().GetProperty(info.Name)?.GetValue(context.Subject) as bool? ?? false);

		private static SelectedMemberInfo GetAsMemberByNumber(IEnumerable<SelectedMemberInfo> selectedMembers, int asMemberNumber)
			=> selectedMembers.SingleOrDefault(info => info.Name == $"{AsTPropertyNamePrefix}{asMemberNumber}");

		private static int GetIsMemberNumber(SelectedMemberInfo isMember)
			=> int.Parse(Regex.Match(isMember.Name, IsTPropertyNameRegex).Groups[1].Value);
	}
}

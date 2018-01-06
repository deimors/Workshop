using FluentAssertions.Equivalency;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FluentAssertions.OneOf
{
	public class ExcludeOneOfAsMembersSelectionRule : IMemberSelectionRule
	{
		public bool IncludesMembers => false;

		public IEnumerable<SelectedMemberInfo> SelectMembers(IEnumerable<SelectedMemberInfo> selectedMembers, ISubjectInfo context, IEquivalencyAssertionOptions config)
			=> IsOneOfType(context.RuntimeType)
				? FilterOneOfTypeMembers(selectedMembers, context)
				: selectedMembers;

		private static IEnumerable<SelectedMemberInfo> FilterOneOfTypeMembers(IEnumerable<SelectedMemberInfo> selectedMembers, ISubjectInfo context)
			=> selectedMembers.Where(info => !IsAsTMember(info));

		private static bool IsOneOfType(Type type)
			=> typeof(IOneOf).IsAssignableFrom(type);

		private static bool IsAsTMember(SelectedMemberInfo info)
			=> Regex.IsMatch(info.Name, @"^AsT\d+$");
	}
}

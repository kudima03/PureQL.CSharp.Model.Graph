using PureQL.CSharp.Model.Samples.Queries.Aggregates;
using PureQL.CSharp.Model.Samples.Queries.Conversions;
using PureQL.CSharp.Model.Samples.Queries.Dashboards;
using PureQL.CSharp.Model.Samples.Queries.Expressions;
using PureQL.CSharp.Model.Samples.Queries.GroupBy;
using PureQL.CSharp.Model.Samples.Queries.Joins;
using PureQL.CSharp.Model.Samples.Queries.Nulls;
using PureQL.CSharp.Model.Samples.Queries.Numbers;
using PureQL.CSharp.Model.Samples.Queries.OrderBy;
using PureQL.CSharp.Model.Samples.Queries.Pagination;
using PureQL.CSharp.Model.Samples.Queries.Parameters;
using PureQL.CSharp.Model.Samples.Queries.Recursive;
using PureQL.CSharp.Model.Samples.Queries.Select;
using PureQL.CSharp.Model.Samples.Queries.SetOperations;
using PureQL.CSharp.Model.Samples.Queries.Strings;
using PureQL.CSharp.Model.Samples.Queries.Subqueries;
using PureQL.CSharp.Model.Samples.Queries.Temporal;
using PureQL.CSharp.Model.Samples.Queries.TimeZones;
using PureQL.CSharp.Model.Samples.Queries.Where;
using PureQL.CSharp.Model.Samples.Queries.Windows;

namespace PureQL.CSharp.Model.Graph.Tests;

/// <summary>
/// Every query of <c>PureQL.CSharp.Model.Samples</c>, listed without reflection so that
/// the Native AOT check runs them all; <c>SampleRoundTripTests</c> checks that the list
/// matches the package.
/// </summary>
internal static class SampleQueries
{
    public static IReadOnlyList<(Type Type, PureQLQuery Query)> All { get; } =
    [
        (typeof(AddedAmountsInATotalQuery), new AddedAmountsInATotalQuery().Value),
        (
            typeof(AggregateEdgesOverAllRowsQuery),
            new AggregateEdgesOverAllRowsQuery().Value
        ),
        (typeof(AggregatesOverAllRowsQuery), new AggregatesOverAllRowsQuery().Value),
        (
            typeof(AggregatesOverExactValuesQuery),
            new AggregatesOverExactValuesQuery().Value
        ),
        (
            typeof(AggregateWithPredicateNextToFieldsQuery),
            new AggregateWithPredicateNextToFieldsQuery().Value
        ),
        (typeof(ConcatAllInATotalQuery), new ConcatAllInATotalQuery().Value),
        (
            typeof(ConcatAllNamesPerCountryQuery),
            new ConcatAllNamesPerCountryQuery().Value
        ),
        (typeof(ConcatAllPerTenantQuery), new ConcatAllPerTenantQuery().Value),
        (
            typeof(ConcatAllRepeatedOnEveryRowQuery),
            new ConcatAllRepeatedOnEveryRowQuery().Value
        ),
        (typeof(CountDistinctInATotalQuery), new CountDistinctInATotalQuery().Value),
        (typeof(CountDistinctPerStatusQuery), new CountDistinctPerStatusQuery().Value),
        (
            typeof(CountDistinctSelectorKindsQuery),
            new CountDistinctSelectorKindsQuery().Value
        ),
        (
            typeof(CovarianceAndCorrelationPerTenantQuery),
            new CovarianceAndCorrelationPerTenantQuery().Value
        ),
        (typeof(DailyRevenueTrendQuery), new DailyRevenueTrendQuery().Value),
        (
            typeof(EveryOperatorSignatureInATotalQuery),
            new EveryOperatorSignatureInATotalQuery().Value
        ),
        (typeof(FirstAndLastInATotalQuery), new FirstAndLastInATotalQuery().Value),
        (
            typeof(FirstAndLastPerStatusOverAllRowsQuery),
            new FirstAndLastPerStatusOverAllRowsQuery().Value
        ),
        (typeof(FirstAndLastPerUserQuery), new FirstAndLastPerUserQuery().Value),
        (typeof(FoldCaseInATotalQuery), new FoldCaseInATotalQuery().Value),
        (typeof(FromPartsInATotalQuery), new FromPartsInATotalQuery().Value),
        (
            typeof(InclusiveComparisonsInATotalQuery),
            new InclusiveComparisonsInATotalQuery().Value
        ),
        (
            typeof(LatestOrderNextToEachOrderQuery),
            new LatestOrderNextToEachOrderQuery().Value
        ),
        (typeof(OrderKpisInOneRowQuery), new OrderKpisInOneRowQuery().Value),
        (typeof(PairStatisticsInATotalQuery), new PairStatisticsInATotalQuery().Value),
        (
            typeof(PairStatisticsOperandKindsQuery),
            new PairStatisticsOperandKindsQuery().Value
        ),
        (
            typeof(PriceStatisticsPerProductQuery),
            new PriceStatisticsPerProductQuery().Value
        ),
        (
            typeof(RatiosGuardedAgainstZeroDivisorsPerProductQuery),
            new RatiosGuardedAgainstZeroDivisorsPerProductQuery().Value
        ),
        (typeof(RatiosInATotalQuery), new RatiosInATotalQuery().Value),
        (typeof(RegressionInAGroupedQuery), new RegressionInAGroupedQuery().Value),
        (typeof(RegressionInATotalQuery), new RegressionInATotalQuery().Value),
        (typeof(RegressionOperandKindsQuery), new RegressionOperandKindsQuery().Value),
        (
            typeof(ScoresAgainstTheDistributionQuery),
            new ScoresAgainstTheDistributionQuery().Value
        ),
        (typeof(ShareOfTotalQuery), new ShareOfTotalQuery().Value),
        (typeof(StatisticsForEveryTypeQuery), new StatisticsForEveryTypeQuery().Value),
        (
            typeof(StatisticsFromAGroupedSubqueryQuery),
            new StatisticsFromAGroupedSubqueryQuery().Value
        ),
        (typeof(StatisticsPerStatusQuery), new StatisticsPerStatusQuery().Value),
        (typeof(TemporalAveragesPerUserQuery), new TemporalAveragesPerUserQuery().Value),
        (typeof(TemporalMeansInATotalQuery), new TemporalMeansInATotalQuery().Value),
        (
            typeof(TemporalMeansOfComputedValuesQuery),
            new TemporalMeansOfComputedValuesQuery().Value
        ),
        (
            typeof(TemporalMeansWithPredicatesQuery),
            new TemporalMeansWithPredicatesQuery().Value
        ),
        (typeof(TotalQueryJoinedOnTrueQuery), new TotalQueryJoinedOnTrueQuery().Value),
        (
            typeof(TrigonometryOverAggregatesInATotalQuery),
            new TrigonometryOverAggregatesInATotalQuery().Value
        ),
        (
            typeof(AsDecimalOfIntegersAndStringsInATotalQuery),
            new AsDecimalOfIntegersAndStringsInATotalQuery().Value
        ),
        (typeof(AsStringOfEveryTypeQuery), new AsStringOfEveryTypeQuery().Value),
        (
            typeof(AsStringOfOperatorResultsQuery),
            new AsStringOfOperatorResultsQuery().Value
        ),
        (typeof(ConversionsInATotalQuery), new ConversionsInATotalQuery().Value),
        (typeof(ConversionsQuery), new ConversionsQuery().Value),
        (
            typeof(DatesCombinedWithDatetimesQuery),
            new DatesCombinedWithDatetimesQuery().Value
        ),
        (typeof(ExplicitWideningQuery), new ExplicitWideningQuery().Value),
        (typeof(OrdersPerDayOfADatetimeQuery), new OrdersPerDayOfADatetimeQuery().Value),
        (typeof(ReadingTextInATotalQuery), new ReadingTextInATotalQuery().Value),
        (typeof(ReadingTextOfEveryTypeQuery), new ReadingTextOfEveryTypeQuery().Value),
        (typeof(ReadingTextQuery), new ReadingTextQuery().Value),
        (
            typeof(TextKeysJoinedToTypedKeysQuery),
            new TextKeysJoinedToTypedKeysQuery().Value
        ),
        (
            typeof(UnparsedPostalCodesPerCountryQuery),
            new UnparsedPostalCodesPerCountryQuery().Value
        ),
        (
            typeof(WideningInGroupKeysAndAggregatesQuery),
            new WideningInGroupKeysAndAggregatesQuery().Value
        ),
        (
            typeof(BilledWithFeePerEmployeeQuery),
            new BilledWithFeePerEmployeeQuery().Value
        ),
        (
            typeof(DailyInStockRevenueShareQuery),
            new DailyInStockRevenueShareQuery().Value
        ),
        (typeof(DailyRevenuePerTenantQuery), new DailyRevenuePerTenantQuery().Value),
        (typeof(DirectReportsPerManagerQuery), new DirectReportsPerManagerQuery().Value),
        (typeof(EmployeeScorecardQuery), new EmployeeScorecardQuery().Value),
        (
            typeof(InStockItemValueVersusOrderedPerUserQuery),
            new InStockItemValueVersusOrderedPerUserQuery().Value
        ),
        (
            typeof(ItemRevenueToOrderTotalPerEmployeeQuery),
            new ItemRevenueToOrderTotalPerEmployeeQuery().Value
        ),
        (
            typeof(ItemValuePerUserAndProductQuery),
            new ItemValuePerUserAndProductQuery().Value
        ),
        (
            typeof(ItemValueShareOfOrderTotalPerProductQuery),
            new ItemValueShareOfOrderTotalPerProductQuery().Value
        ),
        (
            typeof(LineFlagsPerUserAndStatusQuery),
            new LineFlagsPerUserAndStatusQuery().Value
        ),
        (typeof(LineRestockSuggestionQuery), new LineRestockSuggestionQuery().Value),
        (typeof(LoginStatsPerTenantQuery), new LoginStatsPerTenantQuery().Value),
        (typeof(OrdersPerWeekdayQuery), new OrdersPerWeekdayQuery().Value),
        (typeof(OrderStageExcludingUserQuery), new OrderStageExcludingUserQuery().Value),
        (
            typeof(OrderTotalsPerStatusAndTenantQuery),
            new OrderTotalsPerStatusAndTenantQuery().Value
        ),
        (
            typeof(PlacedVersusShippedPerDayQuery),
            new PlacedVersusShippedPerDayQuery().Value
        ),
        (typeof(ProductSalesPerformanceQuery), new ProductSalesPerformanceQuery().Value),
        (
            typeof(RecentOrdersPerActiveUserQuery),
            new RecentOrdersPerActiveUserQuery().Value
        ),
        (
            typeof(RepeatBuyerOrderAndItemProfileQuery),
            new RepeatBuyerOrderAndItemProfileQuery().Value
        ),
        (
            typeof(RevenuePerEmployeeAndAgeQuery),
            new RevenuePerEmployeeAndAgeQuery().Value
        ),
        (
            typeof(RevenuePerTeamLeadPerPeriodQuery),
            new RevenuePerTeamLeadPerPeriodQuery().Value
        ),
        (typeof(RevenuePerUserNameQuery), new RevenuePerUserNameQuery().Value),
        (
            typeof(ScoreByProductPerOrderDayQuery),
            new ScoreByProductPerOrderDayQuery().Value
        ),
        (
            typeof(ShippedSharePerUserAndTenantQuery),
            new ShippedSharePerUserAndTenantQuery().Value
        ),
        (typeof(UnbilledOrderAgingQuery), new UnbilledOrderAgingQuery().Value),
        (typeof(UserAccountOverviewQuery), new UserAccountOverviewQuery().Value),
        (typeof(UserFunnelStageQuery), new UserFunnelStageQuery().Value),
        (typeof(UserSalesSummaryQuery), new UserSalesSummaryQuery().Value),
        (typeof(WeeklyUnitsPerProductQuery), new WeeklyUnitsPerProductQuery().Value),
        (typeof(ConcatQuery), new ConcatQuery().Value),
        (
            typeof(DecimalWideningAndDivideQuery),
            new DecimalWideningAndDivideQuery().Value
        ),
        (typeof(DeeplyNestedIfAndEqualQuery), new DeeplyNestedIfAndEqualQuery().Value),
        (typeof(DeepNumericNestingQuery), new DeepNumericNestingQuery().Value),
        (typeof(DeepOperandChainsQuery), new DeepOperandChainsQuery().Value),
        (
            typeof(EveryOperatorSignatureInAPlainSelectQuery),
            new EveryOperatorSignatureInAPlainSelectQuery().Value
        ),
        (typeof(IfColumnQuery), new IfColumnQuery().Value),
        (
            typeof(IfNestedThirtyLevelsOverCountsQuery),
            new IfNestedThirtyLevelsOverCountsQuery().Value
        ),
        (typeof(IntegerArithmeticQuery), new IntegerArithmeticQuery().Value),
        (
            typeof(LineTotalsAfterCouponDiscountQuery),
            new LineTotalsAfterCouponDiscountQuery().Value
        ),
        (
            typeof(OperandsReadFromDeclaredTypesQuery),
            new OperandsReadFromDeclaredTypesQuery().Value
        ),
        (
            typeof(OperatorTypingAtItsEdgesQuery),
            new OperatorTypingAtItsEdgesQuery().Value
        ),
        (
            typeof(RoundingAndIntegerDivisionQuery),
            new RoundingAndIntegerDivisionQuery().Value
        ),
        (typeof(AddedAmountsInAGroupedQuery), new AddedAmountsInAGroupedQuery().Value),
        (typeof(CircularMeanHourPerUserQuery), new CircularMeanHourPerUserQuery().Value),
        (typeof(ComputedGroupKeyQuery), new ComputedGroupKeyQuery().Value),
        (typeof(ConditionalAggregatesQuery), new ConditionalAggregatesQuery().Value),
        (typeof(CountDistinctInAGroupedQuery), new CountDistinctInAGroupedQuery().Value),
        (
            typeof(CouponEffectivenessPerCouponQuery),
            new CouponEffectivenessPerCouponQuery().Value
        ),
        (typeof(DateMathEverywhereQuery), new DateMathEverywhereQuery().Value),
        (
            typeof(DeclaredOperatorTypesInGroupsQuery),
            new DeclaredOperatorTypesInGroupsQuery().Value
        ),
        (
            typeof(DefaultOverInGroupsAndTotalsQuery),
            new DefaultOverInGroupsAndTotalsQuery().Value
        ),
        (
            typeof(EveryOperatorSignatureAsGroupKeysQuery),
            new EveryOperatorSignatureAsGroupKeysQuery().Value
        ),
        (
            typeof(EveryOperatorSignatureInAGroupedSelectQuery),
            new EveryOperatorSignatureInAGroupedSelectQuery().Value
        ),
        (typeof(FromPartsInAGroupedQuery), new FromPartsInAGroupedQuery().Value),
        (
            typeof(GreatestAndLeastOverAggregatesQuery),
            new GreatestAndLeastOverAggregatesQuery().Value
        ),
        (typeof(GroupByCountQuery), new GroupByCountQuery().Value),
        (typeof(GroupByMonthQuery), new GroupByMonthQuery().Value),
        (typeof(GroupByMultipleKeysQuery), new GroupByMultipleKeysQuery().Value),
        (typeof(GroupedQueryAtItsEdgesQuery), new GroupedQueryAtItsEdgesQuery().Value),
        (typeof(GroupedRevenueQuery), new GroupedRevenueQuery().Value),
        (
            typeof(GroupingByNormalizedKeysQuery),
            new GroupingByNormalizedKeysQuery().Value
        ),
        (typeof(GroupingWithoutAccentsQuery), new GroupingWithoutAccentsQuery().Value),
        (typeof(GroupingWithoutCaseQuery), new GroupingWithoutCaseQuery().Value),
        (typeof(GroupKeyUsageQuery), new GroupKeyUsageQuery().Value),
        (typeof(GrowthPerUserQuery), new GrowthPerUserQuery().Value),
        (typeof(HavingBooleanLogicQuery), new HavingBooleanLogicQuery().Value),
        (typeof(HavingCountQuery), new HavingCountQuery().Value),
        (
            typeof(IntegerAndDecimalInGroupsQuery),
            new IntegerAndDecimalInGroupsQuery().Value
        ),
        (
            typeof(NullableGroupKeyAndAggregatesQuery),
            new NullableGroupKeyAndAggregatesQuery().Value
        ),
        (typeof(OrderSizeBucketsQuery), new OrderSizeBucketsQuery().Value),
        (
            typeof(PairStatisticsInAGroupedQuery),
            new PairStatisticsInAGroupedQuery().Value
        ),
        (
            typeof(PriceDeviationPerProductQuery),
            new PriceDeviationPerProductQuery().Value
        ),
        (
            typeof(RoundedAverageAndQuotientPerTenantQuery),
            new RoundedAverageAndQuotientPerTenantQuery().Value
        ),
        (
            typeof(RoundedIntegersAcrossGroupedClausesQuery),
            new RoundedIntegersAcrossGroupedClausesQuery().Value
        ),
        (typeof(TopCustomersInPeriodQuery), new TopCustomersInPeriodQuery().Value),
        (typeof(FullJoinQuery), new FullJoinQuery().Value),
        (
            typeof(FullJoinThenLeftJoinOnNullableKeysQuery),
            new FullJoinThenLeftJoinOnNullableKeysQuery().Value
        ),
        (typeof(InnerJoinQuery), new InnerJoinQuery().Value),
        (typeof(JoinOnNormalizedSkuQuery), new JoinOnNormalizedSkuQuery().Value),
        (typeof(JoinOnNullableKeyQuery), new JoinOnNullableKeyQuery().Value),
        (typeof(LeftJoinNullableFieldsQuery), new LeftJoinNullableFieldsQuery().Value),
        (typeof(ManagerChainDepthQuery), new ManagerChainDepthQuery().Value),
        (
            typeof(MatchingWithoutCaseOverJoinsQuery),
            new MatchingWithoutCaseOverJoinsQuery().Value
        ),
        (typeof(MultipleJoinsQuery), new MultipleJoinsQuery().Value),
        (typeof(OuterJoinChainQuery), new OuterJoinChainQuery().Value),
        (
            typeof(QuadrantsOfTwoInlineListsQuery),
            new QuadrantsOfTwoInlineListsQuery().Value
        ),
        (typeof(RightJoinAfterLeftJoinsQuery), new RightJoinAfterLeftJoinsQuery().Value),
        (typeof(RightJoinQuery), new RightJoinQuery().Value),
        (typeof(SelfJoinsAfterALeftJoinQuery), new SelfJoinsAfterALeftJoinQuery().Value),
        (typeof(SelfJoinThroughAliasQuery), new SelfJoinThroughAliasQuery().Value),
        (typeof(UsersWithoutOrdersQuery), new UsersWithoutOrdersQuery().Value),
        (
            typeof(AbsentAndNullableOperandsQuery),
            new AbsentAndNullableOperandsQuery().Value
        ),
        (typeof(AsNullableOfEveryTypeQuery), new AsNullableOfEveryTypeQuery().Value),
        (
            typeof(BlankValuesOfEveryTypeAsNullQuery),
            new BlankValuesOfEveryTypeAsNullQuery().Value
        ),
        (
            typeof(CoalesceNestedThirtyLevelsDeepQuery),
            new CoalesceNestedThirtyLevelsDeepQuery().Value
        ),
        (typeof(CoalesceQuery), new CoalesceQuery().Value),
        (typeof(ComparingNullableValuesQuery), new ComparingNullableValuesQuery().Value),
        (
            typeof(ConstantOperandsNeverNullableQuery),
            new ConstantOperandsNeverNullableQuery().Value
        ),
        (
            typeof(InclusiveBoundsThatMayBeNullQuery),
            new InclusiveBoundsThatMayBeNullQuery().Value
        ),
        (
            typeof(InclusiveComparisonsInJoinsAndGroupsQuery),
            new InclusiveComparisonsInJoinsAndGroupsQuery().Value
        ),
        (typeof(LiftedOperatorsQuery), new LiftedOperatorsQuery().Value),
        (typeof(LiftedOverEveryOperandQuery), new LiftedOverEveryOperandQuery().Value),
        (
            typeof(NullableBooleanConditionsQuery),
            new NullableBooleanConditionsQuery().Value
        ),
        (typeof(NullableColumnQuery), new NullableColumnQuery().Value),
        (typeof(NullableOperandsByRoleQuery), new NullableOperandsByRoleQuery().Value),
        (
            typeof(NullableRoleOperandsInAGroupedQuery),
            new NullableRoleOperandsInAGroupedQuery().Value
        ),
        (
            typeof(NullableRoleOperandsInATotalQuery),
            new NullableRoleOperandsInATotalQuery().Value
        ),
        (typeof(NullChecksQuery), new NullChecksQuery().Value),
        (
            typeof(NullIfEqualInEveryClauseOfAGroupedQuery),
            new NullIfEqualInEveryClauseOfAGroupedQuery().Value
        ),
        (typeof(NullIfEqualQuery), new NullIfEqualQuery().Value),
        (
            typeof(OrderingComparisonsOfNullsQuery),
            new OrderingComparisonsOfNullsQuery().Value
        ),
        (typeof(TypedNullLiteralsQuery), new TypedNullLiteralsQuery().Value),
        (
            typeof(AbsSignAndTruncateInATotalQuery),
            new AbsSignAndTruncateInATotalQuery().Value
        ),
        (typeof(AbsSignAndTruncateQuery), new AbsSignAndTruncateQuery().Value),
        (typeof(AgeBandsQuery), new AgeBandsQuery().Value),
        (typeof(DecimalRemainderCornersQuery), new DecimalRemainderCornersQuery().Value),
        (
            typeof(DecimalRemaindersInAGroupedQuery),
            new DecimalRemaindersInAGroupedQuery().Value
        ),
        (
            typeof(DecimalRemaindersInATotalQuery),
            new DecimalRemaindersInATotalQuery().Value
        ),
        (typeof(DecimalRemaindersQuery), new DecimalRemaindersQuery().Value),
        (typeof(GreatCircleDistanceQuery), new GreatCircleDistanceQuery().Value),
        (
            typeof(GreatestAndLeastInATotalQuery),
            new GreatestAndLeastInATotalQuery().Value
        ),
        (
            typeof(GreatestAndLeastOfEveryTypeQuery),
            new GreatestAndLeastOfEveryTypeQuery().Value
        ),
        (typeof(GreatestAndLeastQuery), new GreatestAndLeastQuery().Value),
        (typeof(HourAsAnAngleQuery), new HourAsAnAngleQuery().Value),
        (typeof(IntegersUsedAsTheyAreQuery), new IntegersUsedAsTheyAreQuery().Value),
        (typeof(LongOperandListsQuery), new LongOperandListsQuery().Value),
        (typeof(NestedAndFlatOperatorsQuery), new NestedAndFlatOperatorsQuery().Value),
        (typeof(NumericFunctionCornersQuery), new NumericFunctionCornersQuery().Value),
        (
            typeof(NumericFunctionsInEveryClauseQuery),
            new NumericFunctionsInEveryClauseQuery().Value
        ),
        (
            typeof(OperatorsOverAListInEveryContextQuery),
            new OperatorsOverAListInEveryContextQuery().Value
        ),
        (typeof(OperatorsOverAListQuery), new OperatorsOverAListQuery().Value),
        (typeof(OrdersActiveInAPeriodQuery), new OrdersActiveInAPeriodQuery().Value),
        (
            typeof(PowersAndLogarithmsCornersQuery),
            new PowersAndLogarithmsCornersQuery().Value
        ),
        (
            typeof(PowersAndLogarithmsInEveryClauseQuery),
            new PowersAndLogarithmsInEveryClauseQuery().Value
        ),
        (typeof(PowersAndLogarithmsQuery), new PowersAndLogarithmsQuery().Value),
        (
            typeof(RoundAndTruncateKeepTheOperandTypeQuery),
            new RoundAndTruncateKeepTheOperandTypeQuery().Value
        ),
        (
            typeof(RoundedIntegersAsIntegerOperandsQuery),
            new RoundedIntegersAsIntegerOperandsQuery().Value
        ),
        (typeof(RoundHalfToEvenCornersQuery), new RoundHalfToEvenCornersQuery().Value),
        (
            typeof(RoundHalfToEvenInAGroupedQuery),
            new RoundHalfToEvenInAGroupedQuery().Value
        ),
        (typeof(RoundHalfToEvenInATotalQuery), new RoundHalfToEvenInATotalQuery().Value),
        (typeof(RoundHalfToEvenQuery), new RoundHalfToEvenQuery().Value),
        (typeof(RoundingDecimalResultsQuery), new RoundingDecimalResultsQuery().Value),
        (
            typeof(RoundingIntegersToTensAndHundredsQuery),
            new RoundingIntegersToTensAndHundredsQuery().Value
        ),
        (
            typeof(RoundOfAddNestedThirtyLevelsDeepQuery),
            new RoundOfAddNestedThirtyLevelsDeepQuery().Value
        ),
        (typeof(StandardDeviationByHandQuery), new StandardDeviationByHandQuery().Value),
        (typeof(TrigonometricIdentitiesQuery), new TrigonometricIdentitiesQuery().Value),
        (typeof(WholeNumbersQuery), new WholeNumbersQuery().Value),
        (
            typeof(GroupedSortByColumnsAndExpressionsQuery),
            new GroupedSortByColumnsAndExpressionsQuery().Value
        ),
        (
            typeof(OrderByComputedExpressionQuery),
            new OrderByComputedExpressionQuery().Value
        ),
        (typeof(OrderByMultipleKeysQuery), new OrderByMultipleKeysQuery().Value),
        (typeof(OrderBySelectColumnQuery), new OrderBySelectColumnQuery().Value),
        (typeof(FilterOrderAndPageQuery), new FilterOrderAndPageQuery().Value),
        (typeof(FirstPageWithoutSkipQuery), new FirstPageWithoutSkipQuery().Value),
        (typeof(OrdersAfterTheFirstFourQuery), new OrdersAfterTheFirstFourQuery().Value),
        (typeof(OrdersAfterTheLatestQuery), new OrdersAfterTheLatestQuery().Value),
        (
            typeof(PagedOrderHistoryByParametersQuery),
            new PagedOrderHistoryByParametersQuery().Value
        ),
        (
            typeof(PageOfOneFromAliasedSubqueryQuery),
            new PageOfOneFromAliasedSubqueryQuery().Value
        ),
        (
            typeof(PageOfOrdersFromParametersQuery),
            new PageOfOrdersFromParametersQuery().Value
        ),
        (
            typeof(PagesAfterACursorInSubqueriesQuery),
            new PagesAfterACursorInSubqueriesQuery().Value
        ),
        (typeof(UsersAfterACursorQuery), new UsersAfterACursorQuery().Value),
        (
            typeof(ListParametersThatMayBeEmptyQuery),
            new ListParametersThatMayBeEmptyQuery().Value
        ),
        (
            typeof(OrdersWithTaxFromAParameterRowQuery),
            new OrdersWithTaxFromAParameterRowQuery().Value
        ),
        (
            typeof(ParametersInWhereAndSelectQuery),
            new ParametersInWhereAndSelectQuery().Value
        ),
        (typeof(AncestorsQuery), new AncestorsQuery().Value),
        (typeof(BisectionOfARateQuery), new BisectionOfARateQuery().Value),
        (typeof(CalendarOfDaysQuery), new CalendarOfDaysQuery().Value),
        (
            typeof(ContextsOfRecursiveStepsQuery),
            new ContextsOfRecursiveStepsQuery().Value
        ),
        (typeof(DescendantsQuery), new DescendantsQuery().Value),
        (
            typeof(GroupedNextOverTotalAndCombinedSeedsQuery),
            new GroupedNextOverTotalAndCombinedSeedsQuery().Value
        ),
        (
            typeof(HierarchyWalkWithAggregatedStepsQuery),
            new HierarchyWalkWithAggregatedStepsQuery().Value
        ),
        (typeof(PathCountsPerDepthQuery), new PathCountsPerDepthQuery().Value),
        (typeof(RateOfReturnByIterationQuery), new RateOfReturnByIterationQuery().Value),
        (typeof(RecursiveColumnTypesQuery), new RecursiveColumnTypesQuery().Value),
        (
            typeof(RecursiveNextWithJoinsAndAliasesQuery),
            new RecursiveNextWithJoinsAndAliasesQuery().Value
        ),
        (
            typeof(RecursiveSeedsOfEveryKindQuery),
            new RecursiveSeedsOfEveryKindQuery().Value
        ),
        (
            typeof(RecursiveSubqueryReadersQuery),
            new RecursiveSubqueryReadersQuery().Value
        ),
        (typeof(TreeInReadingOrderQuery), new TreeInReadingOrderQuery().Value),
        (typeof(TreeRollUpQuery), new TreeRollUpQuery().Value),
        (typeof(TwoRecursiveSubqueriesQuery), new TwoRecursiveSubqueriesQuery().Value),
        (typeof(WalkThatStopsAtACycleQuery), new WalkThatStopsAtACycleQuery().Value),
        (
            typeof(WidenedSeedColumnsOfARecursionQuery),
            new WidenedSeedColumnsOfARecursionQuery().Value
        ),
        (typeof(DistinctOrderStatusQuery), new DistinctOrderStatusQuery().Value),
        (typeof(FieldsThroughFromAliasQuery), new FieldsThroughFromAliasQuery().Value),
        (typeof(LatestOrderPerUserQuery), new LatestOrderPerUserQuery().Value),
        (
            typeof(LiteralEdgeValuesOfEveryTypeQuery),
            new LiteralEdgeValuesOfEveryTypeQuery().Value
        ),
        (
            typeof(LiteralOfEveryTypeNextToFieldQuery),
            new LiteralOfEveryTypeNextToFieldQuery().Value
        ),
        (
            typeof(LiteralsAtTheEdgesOfEveryFormatQuery),
            new LiteralsAtTheEdgesOfEveryFormatQuery().Value
        ),
        (
            typeof(OneValueListLiteralsOfEveryTypeQuery),
            new OneValueListLiteralsOfEveryTypeQuery().Value
        ),
        (typeof(UserNameColumnQuery), new UserNameColumnQuery().Value),
        (typeof(ActivityTimelineQuery), new ActivityTimelineQuery().Value),
        (
            typeof(CombinedQueriesInASubqueryChainQuery),
            new CombinedQueriesInASubqueryChainQuery().Value
        ),
        (typeof(GroupsWithATotalsRowQuery), new GroupsWithATotalsRowQuery().Value),
        (typeof(PagedSetOperationInputsQuery), new PagedSetOperationInputsQuery().Value),
        (
            typeof(RowCountsOfEverySetOperationQuery),
            new RowCountsOfEverySetOperationQuery().Value
        ),
        (typeof(UnionColumnTypesQuery), new UnionColumnTypesQuery().Value),
        (
            typeof(UsersCommonToSpendersActiveAndNamedQuery),
            new UsersCommonToSpendersActiveAndNamedQuery().Value
        ),
        (typeof(UsersToContactQuery), new UsersToContactQuery().Value),
        (typeof(CodesMatchedByRegexQuery), new CodesMatchedByRegexQuery().Value),
        (typeof(FixedWidthReportLinesQuery), new FixedWidthReportLinesQuery().Value),
        (typeof(FoldCaseCornersQuery), new FoldCaseCornersQuery().Value),
        (typeof(FoldCaseQuery), new FoldCaseQuery().Value),
        (typeof(NormalizeCornersQuery), new NormalizeCornersQuery().Value),
        (typeof(NormalizeInATotalQuery), new NormalizeInATotalQuery().Value),
        (
            typeof(NormalizeMatchingOverJoinsQuery),
            new NormalizeMatchingOverJoinsQuery().Value
        ),
        (typeof(NormalizeQuery), new NormalizeQuery().Value),
        (typeof(OrderLabelsOverJoinsQuery), new OrderLabelsOverJoinsQuery().Value),
        (
            typeof(PaddingAndJoiningCornersQuery),
            new PaddingAndJoiningCornersQuery().Value
        ),
        (
            typeof(PaddingAndJoiningInATotalQuery),
            new PaddingAndJoiningInATotalQuery().Value
        ),
        (typeof(PaddingAndJoiningQuery), new PaddingAndJoiningQuery().Value),
        (
            typeof(RegularExpressionDialectQuery),
            new RegularExpressionDialectQuery().Value
        ),
        (
            typeof(RegularExpressionsInATotalQuery),
            new RegularExpressionsInATotalQuery().Value
        ),
        (typeof(RegularExpressionsQuery), new RegularExpressionsQuery().Value),
        (typeof(RemoveDiacriticsCornersQuery), new RemoveDiacriticsCornersQuery().Value),
        (
            typeof(RemoveDiacriticsInATotalQuery),
            new RemoveDiacriticsInATotalQuery().Value
        ),
        (
            typeof(RemoveDiacriticsMatchingOverJoinsQuery),
            new RemoveDiacriticsMatchingOverJoinsQuery().Value
        ),
        (typeof(RemoveDiacriticsQuery), new RemoveDiacriticsQuery().Value),
        (
            typeof(ReplacePositionSplitPartCornersQuery),
            new ReplacePositionSplitPartCornersQuery().Value
        ),
        (
            typeof(ReplacePositionSplitPartInATotalQuery),
            new ReplacePositionSplitPartInATotalQuery().Value
        ),
        (
            typeof(ReplacePositionSplitPartQuery),
            new ReplacePositionSplitPartQuery().Value
        ),
        (typeof(StringLengthQuery), new StringLengthQuery().Value),
        (typeof(StringMatchingQuery), new StringMatchingQuery().Value),
        (typeof(SubstringQuery), new SubstringQuery().Value),
        (typeof(TrimQuery), new TrimQuery().Value),
        (typeof(UpperLowerQuery), new UpperLowerQuery().Value),
        (
            typeof(UsersPerEmailDomainByRegexQuery),
            new UsersPerEmailDomainByRegexQuery().Value
        ),
        (typeof(UsersPerEmailDomainQuery), new UsersPerEmailDomainQuery().Value),
        (
            typeof(ConcatAllNamesPerOrderFromASubqueryQuery),
            new ConcatAllNamesPerOrderFromASubqueryQuery().Value
        ),
        (
            typeof(DistinctBySubqueryLeftJoinedQuery),
            new DistinctBySubqueryLeftJoinedQuery().Value
        ),
        (
            typeof(FirstOrderWithinThreeDaysQuery),
            new FirstOrderWithinThreeDaysQuery().Value
        ),
        (typeof(FromSubqueryQuery), new FromSubqueryQuery().Value),
        (
            typeof(InlineRowsOfEveryCellTypeQuery),
            new InlineRowsOfEveryCellTypeQuery().Value
        ),
        (typeof(InlineStatusRowsQuery), new InlineStatusRowsQuery().Value),
        (typeof(InSubqueryColumnQuery), new InSubqueryColumnQuery().Value),
        (
            typeof(LatestOrderPerUserFromASubqueryQuery),
            new LatestOrderPerUserFromASubqueryQuery().Value
        ),
        (typeof(LeftJoinSubqueryQuery), new LeftJoinSubqueryQuery().Value),
        (
            typeof(MonthsWithoutOrdersFilledWithZeroQuery),
            new MonthsWithoutOrdersFilledWithZeroQuery().Value
        ),
        (typeof(OrdersAboveUserAverageQuery), new OrdersAboveUserAverageQuery().Value),
        (typeof(OrdersPerListedStatusQuery), new OrdersPerListedStatusQuery().Value),
        (
            typeof(PaginationFormsInSubqueriesQuery),
            new PaginationFormsInSubqueriesQuery().Value
        ),
        (
            typeof(RoundedIntegersFromSubqueriesQuery),
            new RoundedIntegersFromSubqueriesQuery().Value
        ),
        (
            typeof(SetsThatAreEmptyAtExecutionQuery),
            new SetsThatAreEmptyAtExecutionQuery().Value
        ),
        (
            typeof(SortedSubqueriesDistinctAndPagedQuery),
            new SortedSubqueriesDistinctAndPagedQuery().Value
        ),
        (typeof(SubqueryChainQuery), new SubqueryChainQuery().Value),
        (
            typeof(SubqueryColumnsAfterALeftJoinQuery),
            new SubqueryColumnsAfterALeftJoinQuery().Value
        ),
        (typeof(SubqueryPipelineQuery), new SubqueryPipelineQuery().Value),
        (
            typeof(TopSpendersThroughChainedSubqueriesQuery),
            new TopSpendersThroughChainedSubqueriesQuery().Value
        ),
        (
            typeof(AddingAndTakingTheDifferenceQuery),
            new AddingAndTakingTheDifferenceQuery().Value
        ),
        (typeof(AddMonthsQuery), new AddMonthsQuery().Value),
        (typeof(AmountsNamedByTheirUnitQuery), new AmountsNamedByTheirUnitQuery().Value),
        (typeof(AmountsOfEveryKindQuery), new AmountsOfEveryKindQuery().Value),
        (typeof(DateMathQuery), new DateMathQuery().Value),
        (typeof(DatePartsQuery), new DatePartsQuery().Value),
        (typeof(DatesFromPartsQuery), new DatesFromPartsQuery().Value),
        (typeof(DatetimeUtcLiteralsQuery), new DatetimeUtcLiteralsQuery().Value),
        (typeof(DurationsInPartsQuery), new DurationsInPartsQuery().Value),
        (typeof(MonthsBetweenQuery), new MonthsBetweenQuery().Value),
        (typeof(PartsAndBackQuery), new PartsAndBackQuery().Value),
        (
            typeof(PartsLiftedOverEveryPartQuery),
            new PartsLiftedOverEveryPartQuery().Value
        ),
        (
            typeof(ShiftWindowsInBusinessHoursQuery),
            new ShiftWindowsInBusinessHoursQuery().Value
        ),
        (typeof(TimeAndDatetimeMathQuery), new TimeAndDatetimeMathQuery().Value),
        (
            typeof(TrialsEndingWithinThreeDaysQuery),
            new TrialsEndingWithinThreeDaysQuery().Value
        ),
        (
            typeof(ZoneFreeOperatorsInEveryClauseQuery),
            new ZoneFreeOperatorsInEveryClauseQuery().Value
        ),
        (typeof(DatetimeZoneKindsQuery), new DatetimeZoneKindsQuery().Value),
        (typeof(LocalBusinessDayQuery), new LocalBusinessDayQuery().Value),
        (
            typeof(LocalDatesGroupedAndTotalledQuery),
            new LocalDatesGroupedAndTotalledQuery().Value
        ),
        (typeof(LocalDatesInEveryClauseQuery), new LocalDatesInEveryClauseQuery().Value),
        (typeof(LocalDayBucketsQuery), new LocalDayBucketsQuery().Value),
        (typeof(LocalInstantRoundTripsQuery), new LocalInstantRoundTripsQuery().Value),
        (
            typeof(LocalInstantsInEveryClauseQuery),
            new LocalInstantsInEveryClauseQuery().Value
        ),
        (
            typeof(LocalInstantsInGroupedAndTotalQueriesQuery),
            new LocalInstantsInGroupedAndTotalQueriesQuery().Value
        ),
        (typeof(LocalMonthIdentitiesQuery), new LocalMonthIdentitiesQuery().Value),
        (typeof(LocalMonthKindsQuery), new LocalMonthKindsQuery().Value),
        (
            typeof(LocalMonthsInEveryClauseQuery),
            new LocalMonthsInEveryClauseQuery().Value
        ),
        (
            typeof(LocalMonthsInGroupedAndTotalQueriesQuery),
            new LocalMonthsInGroupedAndTotalQueriesQuery().Value
        ),
        (typeof(LocalMonthsQuery), new LocalMonthsQuery().Value),
        (typeof(LocalPartKindsQuery), new LocalPartKindsQuery().Value),
        (typeof(LocalPartsInConditionsQuery), new LocalPartsInConditionsQuery().Value),
        (typeof(LocalPartsInEveryClauseQuery), new LocalPartsInEveryClauseQuery().Value),
        (
            typeof(LocalPartsInGroupedAndTotalQueriesQuery),
            new LocalPartsInGroupedAndTotalQueriesQuery().Value
        ),
        (typeof(LocalPeriodStartKindsQuery), new LocalPeriodStartKindsQuery().Value),
        (
            typeof(LocalPeriodStartsInEveryClauseQuery),
            new LocalPeriodStartsInEveryClauseQuery().Value
        ),
        (
            typeof(LocalPeriodStartsInGroupedAndTotalQueriesQuery),
            new LocalPeriodStartsInGroupedAndTotalQueriesQuery().Value
        ),
        (typeof(LocalTimesAsInstantsQuery), new LocalTimesAsInstantsQuery().Value),
        (typeof(LocalWeekdayAndHourQuery), new LocalWeekdayAndHourQuery().Value),
        (
            typeof(MonthArithmeticOfLocalDatesQuery),
            new MonthArithmeticOfLocalDatesQuery().Value
        ),
        (typeof(MonthsInALocalCalendarQuery), new MonthsInALocalCalendarQuery().Value),
        (
            typeof(OperatorsWithAndWithoutAZoneQuery),
            new OperatorsWithAndWithoutAZoneQuery().Value
        ),
        (typeof(ZoneExpressionKindsQuery), new ZoneExpressionKindsQuery().Value),
        (typeof(ZoneNameFormsQuery), new ZoneNameFormsQuery().Value),
        (
            typeof(BooleanOperatorsNestedThirtyLevelsDeepQuery),
            new BooleanOperatorsNestedThirtyLevelsDeepQuery().Value
        ),
        (
            typeof(ComparisonsOfEveryTypeInEveryClauseQuery),
            new ComparisonsOfEveryTypeInEveryClauseQuery().Value
        ),
        (
            typeof(DecimalRangesAgainstLiteralsParameterAndFieldQuery),
            new DecimalRangesAgainstLiteralsParameterAndFieldQuery().Value
        ),
        (
            typeof(InListParameterAndNotInListLiteralQuery),
            new InListParameterAndNotInListLiteralQuery().Value
        ),
        (typeof(InOverEveryKindOfListQuery), new InOverEveryKindOfListQuery().Value),
        (
            typeof(ListLiteralsInEveryClauseQuery),
            new ListLiteralsInEveryClauseQuery().Value
        ),
        (
            typeof(OptionalFiltersSwitchedOffByNullQuery),
            new OptionalFiltersSwitchedOffByNullQuery().Value
        ),
        (typeof(OptionalListFiltersQuery), new OptionalListFiltersQuery().Value),
        (typeof(StringDateAndTimeRangesQuery), new StringDateAndTimeRangesQuery().Value),
        (typeof(WhereBooleanFieldQuery), new WhereBooleanFieldQuery().Value),
        (typeof(WhereNestedAndOrQuery), new WhereNestedAndOrQuery().Value),
        (typeof(WhereNotEqualAndNotQuery), new WhereNotEqualAndNotQuery().Value),
        (
            typeof(WhereStatusEqualsLiteralQuery),
            new WhereStatusEqualsLiteralQuery().Value
        ),
        (
            typeof(ComputedKeysSelectorsAndDefaultsInWindowsQuery),
            new ComputedKeysSelectorsAndDefaultsInWindowsQuery().Value
        ),
        (
            typeof(CountDistinctOverWindowsQuery),
            new CountDistinctOverWindowsQuery().Value
        ),
        (
            typeof(EveryAggregateOverARangeQuery),
            new EveryAggregateOverARangeQuery().Value
        ),
        (
            typeof(EveryAggregateOverWindowsQuery),
            new EveryAggregateOverWindowsQuery().Value
        ),
        (typeof(FrameOfEveryFormQuery), new FrameOfEveryFormQuery().Value),
        (typeof(FromPartsOverWindowsQuery), new FromPartsOverWindowsQuery().Value),
        (typeof(LastTwelveMonthsPerUserQuery), new LastTwelveMonthsPerUserQuery().Value),
        (typeof(MovingAveragesQuery), new MovingAveragesQuery().Value),
        (
            typeof(PairStatisticsOverWindowsQuery),
            new PairStatisticsOverWindowsQuery().Value
        ),
        (typeof(PartitionTotalsQuery), new PartitionTotalsQuery().Value),
        (typeof(PreviousAndNextOrderQuery), new PreviousAndNextOrderQuery().Value),
        (typeof(RangeDistancesOverDatesQuery), new RangeDistancesOverDatesQuery().Value),
        (
            typeof(RangeDistancesOverNumbersQuery),
            new RangeDistancesOverNumbersQuery().Value
        ),
        (typeof(RangeFramesQuery), new RangeFramesQuery().Value),
        (typeof(RankingQuery), new RankingQuery().Value),
        (typeof(RegressionOverWindowsQuery), new RegressionOverWindowsQuery().Value),
        (
            typeof(RoundHalfToEvenOverWindowsQuery),
            new RoundHalfToEvenOverWindowsQuery().Value
        ),
        (typeof(RowExpressionsInWindowsQuery), new RowExpressionsInWindowsQuery().Value),
        (typeof(RunningTotalOfDailySumsQuery), new RunningTotalOfDailySumsQuery().Value),
        (typeof(RunningTotalsQuery), new RunningTotalsQuery().Value),
        (
            typeof(SortingBySelectWindowColumnsQuery),
            new SortingBySelectWindowColumnsQuery().Value
        ),
        (typeof(StreaksAndFirstValuesQuery), new StreaksAndFirstValuesQuery().Value),
        (
            typeof(TemporalMeansOverWindowsQuery),
            new TemporalMeansOverWindowsQuery().Value
        ),
        (typeof(TopOrdersPerTenantQuery), new TopOrdersPerTenantQuery().Value),
        (typeof(WindowFunctionsByKindQuery), new WindowFunctionsByKindQuery().Value),
        (typeof(WindowsInEveryPositionQuery), new WindowsInEveryPositionQuery().Value),
        (
            typeof(WindowsKeyedByRowExpressionsQuery),
            new WindowsKeyedByRowExpressionsQuery().Value
        ),
        (
            typeof(WindowsOverNullableFieldsQuery),
            new WindowsOverNullableFieldsQuery().Value
        ),
    ];
}

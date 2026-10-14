/**
 * ==============================================================================
 * HỆ THỐNG STYLING TẬP TRUNG TOÀN ỨNG DỤNG (CENTRALIZED APP STYLES)
 * DNTU Smart Personal Expense & Financial Planning Mobile App
 * ==============================================================================
 * Tệp duy nhất quản lý toàn bộ Style của toàn bộ giao diện:
 * - Re-export đầy đủ Design Tokens: COLORS, SPACING, SIZES, SHADOWS
 * - Base Styles dùng chung: commonStyles
 * - Style phân hệ chuyên biệt: authStyles, homeStyles, transactionStyles,
 *                             budgetStyles, reportStyles, profileStyles, componentStyles
 * ==============================================================================
 */

import { StyleSheet } from 'react-native';
import { COLORS, SPACING, SIZES, SHADOWS } from './theme';

// Re-export design tokens để màn hình chỉ cần import từ 1 tệp duy nhất
export { COLORS, SPACING, SIZES, SHADOWS };

/**
 * 1. BASE COMMON STYLES (DÙNG CHUNG TOÀN BỘ MÀN HÌNH)
 */
export const commonStyles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background
  },
  contentContainer: {
    padding: SPACING.md,
    paddingBottom: 80 // Chống bị che bởi thanh điều hướng Bottom Tabs
  },
  scrollContainer: {
    flexGrow: 1,
    backgroundColor: COLORS.background,
    justifyContent: 'center',
    padding: SPACING.lg
  },
  centerContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: COLORS.background
  },
  loadingText: {
    marginTop: SPACING.sm,
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  card: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  cardSmall: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.small
  },
  cardHeaderTitle: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    marginBottom: SPACING.sm
  },
  sectionTitle: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm
  },
  inputWrap: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    paddingHorizontal: SPACING.md,
    backgroundColor: COLORS.surfaceVariant
  },
  input: {
    flex: 1,
    paddingVertical: SPACING.sm + 2,
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  label: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary,
    marginBottom: 6
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    padding: SPACING.lg
  },
  modalBox: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    padding: SPACING.lg,
    ...SHADOWS.card
  },
  modalTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginBottom: SPACING.md
  },
  modalInput: {
    borderWidth: 1,
    borderColor: COLORS.border,
    borderRadius: SIZES.radiusSm,
    padding: SPACING.md,
    fontSize: SIZES.fontSm,
    backgroundColor: COLORS.surfaceVariant,
    color: COLORS.textPrimary
  },
  modalBtnRow: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginTop: SPACING.md
  },
  modalCancel: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    marginRight: SPACING.sm
  },
  modalCancelText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    fontWeight: '600'
  },
  modalSave: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.lg,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm
  },
  modalSaveText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textWhite,
    fontWeight: 'bold'
  }
});

/**
 * 2. AUTHENTICATION STYLES (LOGIN, REGISTER, FORGOT PASSWORD)
 */
export const authStyles = StyleSheet.create({
  ...commonStyles,
  header: {
    alignItems: 'center',
    marginBottom: SPACING.lg
  },
  iconCircle: {
    width: 76,
    height: 76,
    borderRadius: 38,
    backgroundColor: COLORS.surfaceVariant,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: SPACING.sm,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  appTitle: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.primaryDark,
    textAlign: 'center'
  },
  subtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    textAlign: 'center',
    marginTop: 2
  },
  formCard: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusLg,
    padding: SPACING.lg,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.card
  },
  forgotTouch: {
    alignSelf: 'flex-end',
    marginTop: SPACING.xs + 2,
    marginBottom: SPACING.sm
  },
  forgotText: {
    fontSize: SIZES.fontXs,
    color: COLORS.primary,
    fontWeight: '600'
  },
  switchAuthTouch: {
    marginTop: SPACING.md,
    alignItems: 'center',
    paddingVertical: SPACING.xs
  },
  switchAuthText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  highlightText: {
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  bypassLink: {
    marginTop: SPACING.lg,
    alignItems: 'center',
    padding: SPACING.sm
  },
  bypassText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    textDecorationLine: 'underline'
  },
  instructionText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    lineHeight: 20,
    marginBottom: SPACING.md
  },
  stepProgressRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginVertical: SPACING.xs
  },
  stepDot: {
    width: 28,
    height: 28,
    borderRadius: 14,
    backgroundColor: COLORS.surfaceVariant,
    borderWidth: 1,
    borderColor: COLORS.border,
    alignItems: 'center',
    justifyContent: 'center'
  },
  stepDotActive: {
    backgroundColor: COLORS.primary,
    borderColor: COLORS.primary
  },
  stepDotText: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary
  },
  stepDotTextActive: {
    color: COLORS.textWhite
  },
  stepLine: {
    width: 36,
    height: 2,
    backgroundColor: COLORS.border,
    marginHorizontal: 4
  },
  stepLineActive: {
    backgroundColor: COLORS.primary
  },
  stepTitle: {
    fontSize: SIZES.fontSm,
    fontWeight: '700',
    color: COLORS.primary,
    marginTop: SPACING.xs
  },
  stepDesc: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    lineHeight: 20,
    marginBottom: SPACING.md
  },
  otpInput: {
    letterSpacing: 8,
    fontSize: SIZES.fontLg,
    fontWeight: 'bold'
  },
  subActionTouch: {
    marginTop: SPACING.md,
    alignItems: 'center'
  },
  subActionText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: '600'
  },
  backToLoginTouch: {
    marginTop: SPACING.md,
    alignItems: 'center'
  },
  backToLoginText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: '600'
  },
  backToLoginHighlight: {
    color: COLORS.primary,
    fontWeight: 'bold'
  }
});

/**
 * 3. HOME SCREEN STYLES
 */
export const homeStyles = StyleSheet.create({
  ...commonStyles,
  screenWrapper: {
    flex: 1,
    backgroundColor: '#F8FAFC'
  },
  scrollContent: {
    paddingBottom: 24
  },
  headerArea: {
    backgroundColor: '#1E3A5F',
    paddingHorizontal: 16,
    paddingTop: 16,
    paddingBottom: 28,
    borderBottomLeftRadius: 24,
    borderBottomRightRadius: 24
  },
  topBar: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  header: {
    backgroundColor: '#1E3A5F',
    paddingHorizontal: 16,
    paddingTop: 16,
    paddingBottom: 28,
    borderBottomLeftRadius: 24,
    borderBottomRightRadius: 24
  },
  headerTopRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  greeting: {
    fontSize: 20,
    fontWeight: '800',
    color: '#FFFFFF'
  },
  subGreeting: {
    fontSize: 13,
    color: 'rgba(255, 255, 255, 0.8)',
    marginTop: 2
  },
  bellTouch: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.15)',
    justifyContent: 'center',
    alignItems: 'center'
  },
  balanceCard: {
    backgroundColor: '#FFFFFF',
    marginHorizontal: 16,
    marginTop: -16,
    borderRadius: 18,
    padding: 16,
    borderWidth: 1,
    borderColor: '#E2E8F0',
    ...SHADOWS.card
  },
  balanceLabel: {
    fontSize: 11,
    fontWeight: '700',
    color: '#64748B',
    letterSpacing: 0.8
  },
  balanceAmount: {
    fontSize: 26,
    fontWeight: '900',
    color: '#0F172A',
    marginVertical: 4,
    letterSpacing: 0.3
  },
  rowOverview: {
    flexDirection: 'row',
    marginTop: 10,
    paddingTop: 10,
    borderTopWidth: 1,
    borderTopColor: '#F1F5F9'
  },
  overviewItem: {
    flex: 1
  },
  overviewLabelRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  divider: {
    width: 1,
    backgroundColor: '#E2E8F0',
    marginHorizontal: 12
  },
  incomeLabel: {
    fontSize: 12,
    color: '#10B981',
    fontWeight: '600'
  },
  incomeText: {
    fontSize: 15,
    fontWeight: '800',
    color: '#10B981',
    marginTop: 2
  },
  expenseLabel: {
    fontSize: 12,
    color: '#EF4444',
    fontWeight: '600'
  },
  expenseText: {
    fontSize: 15,
    fontWeight: '800',
    color: '#EF4444',
    marginTop: 2
  },
  quickActionsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingHorizontal: 16,
    marginTop: 16
  },
  quickActionBtn: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: 10,
    marginHorizontal: 4,
    backgroundColor: '#FFFFFF',
    borderRadius: 14,
    borderWidth: 1,
    borderColor: '#F1F5F9',
    ...SHADOWS.small
  },
  quickActionIcon: {
    width: 40,
    height: 40,
    borderRadius: 20,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 6
  },
  quickActionText: {
    fontSize: 11,
    fontWeight: '700',
    color: '#334155'
  },
  sectionBlock: {
    marginTop: 20,
    paddingHorizontal: 16
  },
  sectionHeaderRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 10
  },
  sectionTitle: {
    fontSize: 17,
    fontWeight: '800',
    color: '#0F172A'
  },
  sectionSubtitle: {
    fontSize: 12,
    color: '#64748B'
  },
  viewAllText: {
    fontSize: 13,
    color: '#2563EB',
    fontWeight: '700'
  },
  walletScroll: {
    paddingVertical: 4
  },
  budgetWrapper: {
    marginTop: 18,
    paddingHorizontal: 16
  },
  emptyContainer: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 24,
    backgroundColor: '#FFFFFF',
    borderRadius: 14,
    borderWidth: 1,
    borderColor: '#F1F5F9'
  },
  emptyText: {
    fontSize: 13,
    color: '#94A3B8',
    marginTop: 6
  }
});

/**
 * 4. TRANSACTION STYLES (LIST & ADD/EDIT SCREEN & ITEM)
 */
export const transactionStyles = StyleSheet.create({
  ...commonStyles,
  topFilterBar: {
    paddingHorizontal: SPACING.md,
    paddingTop: SPACING.md,
    paddingBottom: SPACING.xs
  },
  searchBox: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    paddingHorizontal: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    height: 44,
    ...SHADOWS.small
  },
  monthNavigator: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusSm,
    paddingHorizontal: SPACING.sm,
    paddingVertical: SPACING.xs,
    marginTop: SPACING.xs + 2,
    borderWidth: 1,
    borderColor: COLORS.borderLight
  },
  monthNavBtn: {
    padding: SPACING.xs,
    borderRadius: SIZES.radiusSm,
    backgroundColor: COLORS.surfaceVariant
  },
  monthLabelWrap: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  monthLabelText: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.primaryDark
  },
  tabContainer: {
    flexDirection: 'row',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    backgroundColor: COLORS.background
  },
  tabButton: {
    flex: 1,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm,
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    marginHorizontal: 3
  },
  tabButtonActive: {
    backgroundColor: COLORS.primary
  },
  tabExpenseActive: {
    backgroundColor: COLORS.expense
  },
  tabIncomeActive: {
    backgroundColor: COLORS.income
  },
  tabText: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textSecondary
  },
  tabTextActive: {
    color: COLORS.textWhite
  },
  summaryBar: {
    flexDirection: 'row',
    backgroundColor: COLORS.surface,
    marginHorizontal: SPACING.md,
    marginBottom: SPACING.xs,
    paddingVertical: SPACING.sm,
    paddingHorizontal: SPACING.md,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.borderLight,
    alignItems: 'center',
    justifyContent: 'space-around'
  },
  summaryItem: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  summaryLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginRight: 6
  },
  summaryExpense: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.expense
  },
  summaryIncome: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: COLORS.income
  },
  summaryDivider: {
    width: 1,
    height: 18,
    backgroundColor: COLORS.border
  },
  listContent: {
    paddingHorizontal: SPACING.md,
    paddingTop: SPACING.xs,
    paddingBottom: 80
  },
  emptyContainer: {
    paddingVertical: SPACING.xxl,
    alignItems: 'center',
    justifyContent: 'center'
  },
  emptyTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginTop: SPACING.sm
  },
  emptySubtitle: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 4,
    textAlign: 'center'
  },
  emptyAddBtn: {
    marginTop: SPACING.md,
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.lg,
    paddingVertical: SPACING.sm + 2,
    borderRadius: SIZES.radiusMd
  },
  emptyAddBtnText: {
    color: COLORS.textWhite,
    fontSize: SIZES.fontSm,
    fontWeight: 'bold'
  },
  filterSection: {
    backgroundColor: COLORS.surface,
    padding: SPACING.sm,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.sm,
    ...SHADOWS.small
  },
  searchBar: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusSm,
    paddingHorizontal: SPACING.sm,
    paddingVertical: 6,
    marginBottom: SPACING.xs
  },
  searchInput: {
    flex: 1,
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary,
    marginLeft: SPACING.xs
  },
  filterPillRow: {
    flexDirection: 'row',
    marginTop: 4
  },
  filterPill: {
    paddingHorizontal: SPACING.md,
    paddingVertical: 6,
    borderRadius: SIZES.radiusFull,
    backgroundColor: COLORS.surfaceVariant,
    marginRight: SPACING.xs,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  filterPillActive: {
    backgroundColor: COLORS.primary,
    borderColor: COLORS.primary
  },
  filterPillText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    fontWeight: '600'
  },
  filterPillTextActive: {
    color: COLORS.textWhite
  },
  itemCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    padding: SPACING.sm + 4,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.xs + 2,
    ...SHADOWS.small
  },
  iconBox: {
    width: 44,
    height: 44,
    borderRadius: SIZES.radiusSm,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: SPACING.sm
  },
  itemDetails: {
    flex: 1,
    justifyContent: 'center'
  },
  itemCategory: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary
  },
  itemNote: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginTop: 1
  },
  itemMeta: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.textMuted,
    marginTop: 2
  },
  itemRight: {
    alignItems: 'flex-end',
    marginLeft: SPACING.xs
  },
  itemAmount: {
    fontSize: SIZES.fontSm + 1,
    fontWeight: 'bold'
  },
  deleteBtn: {
    padding: 4,
    marginTop: 2
  },
  tabSwitch: {
    flexDirection: 'row',
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusMd,
    padding: 4,
    marginBottom: SPACING.md
  },
  switchButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: SPACING.sm + 2,
    borderRadius: SIZES.radiusSm
  },
  switchExpenseActive: {
    backgroundColor: COLORS.expense
  },
  switchIncomeActive: {
    backgroundColor: COLORS.income
  },
  switchText: {
    fontSize: SIZES.fontMd,
    fontWeight: '600',
    color: COLORS.textSecondary
  },
  switchTextActive: {
    color: COLORS.textWhite
  },
  amountCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  cardSectionLabel: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    marginBottom: SPACING.xs
  },
  amountInput: {
    fontSize: SIZES.fontDisplay,
    fontWeight: 'bold',
    textAlign: 'center',
    width: '100%',
    paddingVertical: SPACING.xs
  },
  expenseTextColor: {
    color: COLORS.expense
  },
  incomeTextColor: {
    color: COLORS.income
  },
  amountPreviewText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  quickAmountRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center',
    marginTop: SPACING.sm,
    paddingTop: SPACING.xs,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight,
    width: '100%'
  },
  quickChip: {
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: SIZES.radiusSm,
    backgroundColor: COLORS.surfaceVariant,
    marginHorizontal: 3,
    marginVertical: 3,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  quickChipText: {
    fontSize: SIZES.fontXs,
    fontWeight: '700',
    color: COLORS.textPrimary
  },
  quickChipClear: {
    backgroundColor: '#FEE2E2',
    borderColor: '#FCA5A5'
  },
  quickChipClearText: {
    color: COLORS.danger
  },
  sectionCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.small
  },
  walletRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: SPACING.xs
  },
  walletChip: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.border,
    backgroundColor: COLORS.surfaceVariant,
    marginRight: SPACING.sm,
    marginBottom: SPACING.xs
  },
  walletChipSelected: {
    borderColor: COLORS.primary,
    backgroundColor: COLORS.surface
  },
  walletChipText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  walletChipTextSelected: {
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  gridContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: SPACING.xs
  },
  categoryGridItem: {
    width: '23%',
    paddingVertical: 10,
    margin: '1%',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.borderLight,
    backgroundColor: COLORS.surface
  },
  categoryIconCircle: {
    width: 38,
    height: 38,
    borderRadius: 19,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: 4
  },
  categoryNameText: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.textSecondary,
    textAlign: 'center',
    paddingHorizontal: 2
  },
  dateChipGroup: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  dateChip: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6,
    backgroundColor: COLORS.surface,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginLeft: 4
  },
  dateChipText: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.primary,
    fontWeight: '600'
  },
  inputRow: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusSm,
    paddingHorizontal: SPACING.sm,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  textInput: {
    flex: 1,
    paddingVertical: SPACING.sm,
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  buttonWrapper: {
    marginTop: SPACING.sm
  }
});

/**
 * 5. BUDGET SCREEN & BAR STYLES
 */
export const budgetStyles = StyleSheet.create({
  ...commonStyles,
  mainCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    overflow: 'hidden',
    ...SHADOWS.card
  },
  alertBox: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: SPACING.sm + 4,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    marginTop: SPACING.md
  },
  alertText: {
    flex: 1,
    fontSize: SIZES.fontSm,
    fontWeight: '600'
  },
  alertSubText: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  editLimitBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: SPACING.md,
    paddingVertical: SPACING.sm,
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusSm
  },
  editLimitText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  metricGrid: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: SPACING.md
  },
  metricCard: {
    width: '48%',
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.small
  },
  metricIconWrap: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: COLORS.surfaceVariant,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: SPACING.xs
  },
  metricLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    marginBottom: 2
  },
  metricValue: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  metricSub: {
    fontSize: SIZES.fontXs - 2,
    color: COLORS.textMuted,
    marginTop: 2
  },
  sectionHeaderRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.sm,
    marginTop: SPACING.xs
  },
  sectionTitle: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  sectionSubtitle: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  catBudgetCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.sm,
    ...SHADOWS.small
  },
  catHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.xs
  },
  catTitleRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  catDot: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 8
  },
  catName: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary
  },
  catPercent: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold'
  },
  catProgressTrack: {
    height: 8,
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: 4,
    overflow: 'hidden',
    marginVertical: 4
  },
  catProgressFill: {
    height: '100%',
    borderRadius: 4
  },
  catFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 4
  },
  catFooterText: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  modalContent: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusLg,
    padding: SPACING.lg
  },
  modalSub: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginBottom: SPACING.md
  },
  modalPreviewText: {
    fontSize: SIZES.fontSm,
    color: COLORS.primary,
    fontWeight: '600',
    marginBottom: SPACING.sm
  },
  presetChipRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginBottom: SPACING.md
  },
  presetChip: {
    backgroundColor: COLORS.surfaceVariant,
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginRight: 6,
    marginBottom: 6
  },
  presetChipText: {
    fontSize: SIZES.fontXs,
    color: COLORS.primaryDark,
    fontWeight: '600'
  },
  modalCancelBtn: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    marginRight: SPACING.sm
  },
  modalCancelText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    fontWeight: '600'
  },
  modalSaveBtn: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.lg,
    paddingVertical: SPACING.sm,
    borderRadius: SIZES.radiusSm
  },
  modalSaveText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textWhite,
    fontWeight: 'bold'
  },
  cardContainer: {
    width: '100%',
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    padding: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    ...SHADOWS.card
  },
  plainContainer: {
    width: '100%',
    backgroundColor: 'transparent',
    padding: 0
  },
  headerRow: {
    width: '100%',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SPACING.sm
  },
  titleGroup: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: 8
  },
  title: {
    fontSize: 14,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    flex: 1
  },
  badgePill: {
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0
  },
  percentBadge: {
    fontSize: 12,
    fontWeight: '700'
  },
  track: {
    width: '100%',
    height: 10,
    backgroundColor: '#EEEEEE',
    borderRadius: SIZES.radiusFull,
    overflow: 'hidden',
    marginBottom: SPACING.xs
  },
  fill: {
    height: '100%',
    borderRadius: SIZES.radiusFull
  },
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SPACING.xs
  },
  spentText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  limitText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  statusMsg: {
    fontSize: SIZES.fontSm - 1,
    fontWeight: '500',
    marginTop: 6
  },
  adviceCard: {
    backgroundColor: '#EFF6FF',
    padding: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: '#BFDBFE',
    marginTop: SPACING.sm
  },
  adviceTitle: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: '#1E40AF',
    marginBottom: 4
  },
  adviceContent: {
    fontSize: SIZES.fontXs,
    color: '#1E3A8A',
    lineHeight: 18
  },
  setBudgetCard: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    padding: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginTop: SPACING.md,
    ...SHADOWS.small
  }
});

/**
 * 6. REPORT SCREEN STYLES
 */
export const reportStyles = StyleSheet.create({
  ...commonStyles,
  summaryCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  summaryRow: {
    flexDirection: 'row',
    paddingBottom: SPACING.sm,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.borderLight
  },
  summaryCol: {
    flex: 1
  },
  labelRow: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  incomeLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.income,
    fontWeight: '600'
  },
  incomeAmount: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.income,
    marginTop: 2
  },
  expenseLabel: {
    fontSize: SIZES.fontSm,
    color: COLORS.expense,
    fontWeight: '600'
  },
  expenseAmount: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.expense,
    marginTop: 2
  },
  summaryDivider: {
    width: 1,
    backgroundColor: COLORS.borderLight,
    marginHorizontal: SPACING.sm
  },
  netBalanceRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SPACING.sm
  },
  netLabel: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  netAmount: {
    fontSize: SIZES.fontLg,
    fontWeight: 'bold',
    marginTop: 2
  },
  savingsBadge: {
    backgroundColor: '#E0F2FE',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.xs + 2,
    borderRadius: SIZES.radiusFull
  },
  savingsBadgeText: {
    fontSize: SIZES.fontSm,
    fontWeight: 'bold',
    color: '#0284C7'
  },
  chartCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    alignItems: 'center',
    ...SHADOWS.small
  },
  chartHeaderRow: {
    flexDirection: 'row',
    alignItems: 'center',
    alignSelf: 'flex-start',
    marginBottom: 4
  },
  chartTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  unitHint: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary,
    alignSelf: 'flex-start',
    marginBottom: SPACING.sm
  },
  barChartStyle: {
    marginVertical: 4,
    borderRadius: SIZES.radiusMd
  },
  detailCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.md,
    borderRadius: SIZES.radiusLg,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.small
  },
  detailCardTitle: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary,
    marginBottom: SPACING.sm
  },
  detailRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: SPACING.xs + 2,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.borderLight
  },
  detailLeft: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  detailDot: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 8
  },
  detailName: {
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary
  },
  detailRight: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  detailAmount: {
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary,
    marginRight: 4
  },
  detailPct: {
    fontSize: SIZES.fontXs,
    color: COLORS.textSecondary
  },
  chartEmptyWrap: {
    paddingVertical: SPACING.xl,
    alignItems: 'center',
    justifyContent: 'center',
    width: '100%'
  },
  chartEmptyText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: SPACING.xs + 2,
    textAlign: 'center'
  },
  emptyActionBtn: {
    marginTop: SPACING.sm,
    backgroundColor: COLORS.surfaceVariant,
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.xs + 2,
    borderRadius: SIZES.radiusSm,
    borderWidth: 1,
    borderColor: COLORS.border
  },
  emptyActionBtnText: {
    fontSize: SIZES.fontSm - 1,
    color: COLORS.primary,
    fontWeight: 'bold'
  },
  chartToggleRow: {
    flexDirection: 'row',
    backgroundColor: COLORS.surfaceVariant,
    borderRadius: SIZES.radiusSm,
    padding: 3,
    marginBottom: SPACING.sm,
    alignSelf: 'stretch'
  },
  chartToggleBtn: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 6,
    borderRadius: SIZES.radiusSm - 2
  },
  chartToggleBtnActive: {
    backgroundColor: COLORS.primary,
    ...SHADOWS.small
  },
  chartToggleBtnActiveIncome: {
    backgroundColor: COLORS.income,
    ...SHADOWS.small
  },
  chartToggleText: {
    fontSize: SIZES.fontXs,
    fontWeight: '600',
    color: COLORS.textSecondary
  },
  chartToggleTextActive: {
    color: '#FFFFFF',
    fontWeight: 'bold'
  },
  historyTable: {
    marginTop: SPACING.xs,
    width: '100%'
  },
  historyTableRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: COLORS.borderLight
  },
  historyTableColMonth: {
    width: '28%',
    fontSize: SIZES.fontSm,
    fontWeight: '600',
    color: COLORS.textPrimary
  },
  historyTableColIncome: {
    width: '24%',
    fontSize: SIZES.fontXs + 1,
    fontWeight: '600',
    color: COLORS.income,
    textAlign: 'right'
  },
  historyTableColExpense: {
    width: '24%',
    fontSize: SIZES.fontXs + 1,
    fontWeight: '600',
    color: COLORS.expense,
    textAlign: 'right'
  },
  historyTableColNet: {
    width: '24%',
    fontSize: SIZES.fontXs + 1,
    fontWeight: 'bold',
    textAlign: 'right'
  }
});

/**
 * 7. PROFILE SCREEN STYLES
 */
export const profileStyles = StyleSheet.create({
  ...commonStyles,
  profileCard: {
    backgroundColor: COLORS.surface,
    padding: SPACING.lg,
    borderRadius: SIZES.radiusLg,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    ...SHADOWS.card
  },
  avatarCircle: {
    width: 72,
    height: 72,
    borderRadius: 36,
    backgroundColor: COLORS.primary,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: SPACING.sm
  },
  avatarText: {
    fontSize: SIZES.fontXxl,
    fontWeight: 'bold',
    color: COLORS.textWhite
  },
  profileName: {
    fontSize: SIZES.fontXl,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  profileEmail: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  badgeRow: {
    marginTop: SPACING.xs,
    marginBottom: SPACING.md
  },
  roleBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.surfaceVariant,
    paddingHorizontal: SPACING.md,
    paddingVertical: 4,
    borderRadius: SIZES.radiusFull
  },
  roleBadgeText: {
    fontSize: SIZES.fontXs,
    color: COLORS.primary,
    fontWeight: '600'
  },
  statsRow: {
    flexDirection: 'row',
    width: '100%',
    paddingTop: SPACING.md,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight,
    justifyContent: 'space-around'
  },
  statCol: {
    alignItems: 'center'
  },
  statVal: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.textPrimary
  },
  statLbl: {
    fontSize: SIZES.fontXs - 2,
    color: COLORS.textSecondary,
    marginTop: 2
  },
  statDivider: {
    width: 1,
    height: 24,
    backgroundColor: COLORS.borderLight
  },
  menuSection: {
    backgroundColor: COLORS.surface,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: COLORS.border,
    marginBottom: SPACING.md,
    paddingVertical: SPACING.xs,
    ...SHADOWS.small
  },
  menuSectionTitle: {
    fontSize: SIZES.fontXs,
    fontWeight: 'bold',
    color: COLORS.textSecondary,
    letterSpacing: 0.5,
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm + 4,
    borderTopWidth: 1,
    borderTopColor: COLORS.borderLight
  },
  menuLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1
  },
  menuIconWrap: {
    width: 32,
    height: 32,
    borderRadius: 8,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: SPACING.md
  },
  menuText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textPrimary,
    fontWeight: '500'
  },
  menuSubText: {
    fontSize: SIZES.fontXs - 1,
    color: COLORS.textSecondary,
    marginTop: 1
  },
  logoutBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#FEE2E2',
    paddingVertical: SPACING.md,
    borderRadius: SIZES.radiusMd,
    borderWidth: 1,
    borderColor: '#FCA5A5',
    marginTop: SPACING.xs
  },
  logoutText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold',
    color: COLORS.danger
  }
});

/**
/**
 * 8. COMPONENT SPECIFIC STYLES
 */

// 8.1. WalletCard Styles
export const walletCardStyles = StyleSheet.create({
  card: {
    width: 195,
    minHeight: 120,
    borderRadius: 16,
    padding: 14,
    marginRight: 12,
    backgroundColor: '#FFFFFF',
    borderWidth: 1,
    borderColor: '#E2E8F0',
    borderLeftWidth: 4,
    justifyContent: 'space-between',
    ...SHADOWS.small
  },
  cardSelected: {
    borderColor: COLORS.primaryDark,
    backgroundColor: COLORS.primary,
    borderLeftColor: '#60A5FA',
    ...SHADOWS.card
  },
  headerRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8
  },
  badgeGroup: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(0, 0, 0, 0.04)',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 12
  },
  iconCircle: {
    width: 34,
    height: 34,
    borderRadius: 17,
    justifyContent: 'center',
    alignItems: 'center'
  },
  iconCircleSelected: {
    backgroundColor: 'rgba(255, 255, 255, 0.25)'
  },
  badgeText: {
    fontSize: 11,
    fontWeight: '700'
  },
  walletName: {
    fontSize: 14,
    fontWeight: '600',
    color: '#334155',
    marginBottom: 4
  },
  walletNameSelected: {
    color: '#E2E8F0'
  },
  balanceText: {
    fontSize: 17,
    fontWeight: '800',
    color: '#0F172A',
    letterSpacing: 0.3
  },
  balanceTextSelected: {
    color: '#FFFFFF'
  }
});

// 8.2. TransactionItem Styles
export const transactionItemStyles = StyleSheet.create({
  card: {
    backgroundColor: '#FFFFFF',
    borderRadius: 14,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#F1F5F9',
    ...SHADOWS.small,
    overflow: 'hidden'
  },
  mainContent: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 12,
    paddingLeft: 14,
    paddingRight: 6
  },
  iconWrapper: {
    width: 44,
    height: 44,
    borderRadius: 14,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12
  },
  centerInfo: {
    flex: 1,
    justifyContent: 'center'
  },
  categoryTitle: {
    fontSize: 15,
    fontWeight: '700',
    color: '#0F172A',
    marginBottom: 2
  },
  noteText: {
    fontSize: 13,
    color: '#64748B',
    marginBottom: 4
  },
  metaRow: {
    flexDirection: 'row',
    alignItems: 'center',
    flexWrap: 'wrap'
  },
  dateText: {
    fontSize: 11,
    color: '#94A3B8',
    marginRight: 6
  },
  walletBadgeContainer: {
    backgroundColor: '#F1F5F9',
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 6
  },
  walletBadge: {
    fontSize: 10,
    color: '#475569',
    fontWeight: '600'
  },
  amountCol: {
    alignItems: 'flex-end',
    justifyContent: 'center',
    paddingLeft: 8
  },
  amountText: {
    fontSize: 15,
    fontWeight: '800',
    letterSpacing: 0.2
  },
  expenseText: {
    color: '#EF4444'
  },
  incomeText: {
    color: '#10B981'
  },
  deleteButton: {
    paddingHorizontal: 12,
    paddingVertical: 18,
    justifyContent: 'center',
    alignItems: 'center'
  }
});

// 8.3. BudgetBar Styles
export const budgetBarStyles = StyleSheet.create({
  cardContainer: {
    width: '100%',
    backgroundColor: COLORS.card,
    borderRadius: SIZES.radiusMd,
    padding: SIZES.md,
    borderWidth: 1,
    borderColor: COLORS.border,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.08,
    shadowRadius: 2
  },
  plainContainer: {
    width: '100%',
    backgroundColor: 'transparent',
    padding: 0
  },
  headerRow: {
    width: '100%',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: SIZES.sm
  },
  titleGroup: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: 8
  },
  title: {
    fontSize: 14,
    fontWeight: 'bold',
    color: COLORS.text,
    flex: 1
  },
  badgePill: {
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0
  },
  percentBadge: {
    fontSize: 12,
    fontWeight: '700'
  },
  track: {
    width: '100%',
    height: 10,
    backgroundColor: '#EEEEEE',
    borderRadius: SIZES.radiusFull,
    overflow: 'hidden',
    marginBottom: SIZES.xs
  },
  fill: {
    height: '100%',
    borderRadius: SIZES.radiusFull
  },
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SIZES.xs
  },
  spentText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  limitText: {
    fontSize: SIZES.fontSm,
    color: COLORS.textSecondary
  },
  statusMsg: {
    fontSize: SIZES.fontSm - 1,
    fontWeight: '500',
    marginTop: 6
  }
});

// 8.4. RealtimeAlertBanner Styles
export const realtimeBannerStyles = StyleSheet.create({
  wrapper: {
    backgroundColor: '#1E293B',
    paddingHorizontal: 16,
    paddingTop: 8,
    paddingBottom: 8,
    borderBottomWidth: 1,
    borderBottomColor: 'rgba(255, 255, 255, 0.08)'
  },
  statusBar: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  statusLeft: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 6
  },
  dotConnected: {
    backgroundColor: '#10B981'
  },
  dotDisconnected: {
    backgroundColor: '#94A3B8'
  },
  statusText: {
    fontSize: 12,
    color: '#E2E8F0',
    fontWeight: '600'
  },
  latencyBadge: {
    marginLeft: 6,
    paddingHorizontal: 6,
    paddingVertical: 1,
    borderRadius: 8,
    backgroundColor: 'rgba(16, 185, 129, 0.15)',
    borderWidth: 0.5,
    borderColor: 'rgba(16, 185, 129, 0.3)'
  },
  latencyText: {
    fontSize: 10,
    color: '#34D399',
    fontWeight: '700'
  },
  testBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(255, 255, 255, 0.12)',
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 14,
    borderWidth: 0.5,
    borderColor: 'rgba(255, 255, 255, 0.2)'
  },
  testBtnText: {
    fontSize: 11,
    color: '#FFFFFF',
    fontWeight: '700'
  },
  alertCard: {
    borderRadius: 10,
    padding: 10,
    marginTop: 8,
    borderWidth: 1
  },
  cardWarning: {
    backgroundColor: '#FFFBEB',
    borderColor: '#FDE68A'
  },
  cardDanger: {
    backgroundColor: '#FEF2F2',
    borderColor: '#FECACA'
  },
  alertHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 2
  },
  alertTitle: {
    flex: 1,
    fontSize: 12,
    fontWeight: '700',
    marginLeft: 6
  },
  textWarning: {
    color: '#B45309'
  },
  textDanger: {
    color: '#B91C1C'
  },
  alertMessage: {
    fontSize: 12,
    color: '#334155',
    lineHeight: 16,
    marginTop: 2
  },
  balanceCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#F0FDF4',
    borderRadius: 10,
    padding: 10,
    marginTop: 8,
    borderWidth: 1,
    borderColor: '#BBF7D0'
  },
  balanceIconWrap: {
    width: 32,
    height: 32,
    borderRadius: 16,
    backgroundColor: '#DCFCE7',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 10
  },
  balanceInfo: {
    flex: 1
  },
  balanceTitle: {
    fontSize: 12,
    color: '#1E293B',
    fontWeight: '600'
  },
  balanceSub: {
    fontSize: 11,
    color: '#64748B',
    marginTop: 1
  },
  closeBtn: {
    padding: 4
  }
});

// 8.5. CustomButton Styles
export const customButtonStyles = StyleSheet.create({
  baseBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: SIZES.md - 2,
    paddingHorizontal: SIZES.lg,
    borderRadius: SIZES.radiusMd,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2
  },
  btnPrimary: {
    backgroundColor: COLORS.primary
  },
  btnDanger: {
    backgroundColor: COLORS.danger
  },
  btnIncome: {
    backgroundColor: COLORS.income
  },
  btnOutline: {
    backgroundColor: 'transparent',
    borderWidth: 1.5,
    borderColor: COLORS.primary,
    elevation: 0
  },
  baseText: {
    fontSize: SIZES.fontMd,
    fontWeight: 'bold'
  },
  textWhite: {
    color: COLORS.white
  },
  textOutline: {
    color: COLORS.primary
  }
});

// 8.6. Consolidated Component Styles (Dùng chung cho toàn bộ components)
export const componentStyles = StyleSheet.create({
  ...commonStyles,
  ...walletCardStyles,
  ...transactionItemStyles,
  ...budgetBarStyles,
  ...realtimeBannerStyles,
  ...customButtonStyles
});

/**
 * DEFAULT EXPORT TỔNG HỢP (CONSOLIDATED APP STYLES)
 */
const appStyles = {
  COLORS,
  SPACING,
  SIZES,
  SHADOWS,
  common: commonStyles,
  auth: authStyles,
  home: homeStyles,
  transaction: transactionStyles,
  budget: budgetStyles,
  report: reportStyles,
  profile: profileStyles,
  component: componentStyles,
  walletCard: walletCardStyles,
  transactionItem: transactionItemStyles,
  budgetBar: budgetBarStyles,
  realtimeBanner: realtimeBannerStyles,
  customButton: customButtonStyles
};

export default appStyles;

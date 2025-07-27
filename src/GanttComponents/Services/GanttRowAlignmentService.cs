namespace GanttComponents.Services;

/// <summary>
/// Row metrics for alignment between TaskGrid and TimelineView
/// </summary>
public record RowMetrics
{
    public int Index { get; set; }
    public int Height { get; set; }
    public int Top { get; set; }
    public bool Visible { get; set; }
    public bool Expanded { get; set; }
    public int TaskId { get; set; }
}

/// <summary>
/// Service for managing row alignment between TaskGrid and TimelineView components
/// Critical for maintaining pixel-perfect alignment in GanttComposer
/// </summary>
public class GanttRowAlignmentService
{
    private Dictionary<int, RowMetrics> _rowPositions = new();
    private int _defaultRowHeight = 32;
    private int _headerHeight = 40;

    /// <summary>
    /// Event fired when row positions change
    /// </summary>
    public event Action<Dictionary<int, RowMetrics>>? RowPositionsChanged;

    /// <summary>
    /// Default row height in pixels
    /// </summary>
    public int DefaultRowHeight
    {
        get => _defaultRowHeight;
        set
        {
            _defaultRowHeight = value;
            RecalculateAllPositions();
        }
    }

    /// <summary>
    /// Header height in pixels
    /// </summary>
    public int HeaderHeight
    {
        get => _headerHeight;
        set
        {
            _headerHeight = value;
            RecalculateAllPositions();
        }
    }

    /// <summary>
    /// Get all current row positions
    /// </summary>
    public Dictionary<int, RowMetrics> RowPositions => new(_rowPositions);

    /// <summary>
    /// Update row position metrics
    /// </summary>
    public void UpdateRowPosition(int index, RowMetrics metrics)
    {
        _rowPositions[index] = metrics;
        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>(_rowPositions));
    }

    /// <summary>
    /// Calculate row positions based on data and expansion states
    /// </summary>
    public Dictionary<int, RowMetrics> CalculateRowPositions(IEnumerable<object> data, Dictionary<int, bool>? expandedStates = null)
    {
        var positions = new Dictionary<int, RowMetrics>();
        var currentTop = _headerHeight;
        var index = 0;

        foreach (var item in data)
        {
            var isExpanded = expandedStates?.GetValueOrDefault(index, true) ?? true;
            var rowHeight = _defaultRowHeight;

            positions[index] = new RowMetrics
            {
                Index = index,
                Height = rowHeight,
                Top = currentTop,
                Visible = true,
                Expanded = isExpanded,
                TaskId = GetTaskId(item)
            };

            currentTop += rowHeight;
            index++;
        }

        _rowPositions = positions;
        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>(_rowPositions));

        return positions;
    }

    /// <summary>
    /// Get row at specific Y position
    /// </summary>
    public RowMetrics? GetRowAtPosition(int y)
    {
        return _rowPositions.Values
            .FirstOrDefault(row => y >= row.Top && y < row.Top + row.Height);
    }

    /// <summary>
    /// Get row metrics by index
    /// </summary>
    public RowMetrics? GetRowByIndex(int index)
    {
        return _rowPositions.GetValueOrDefault(index);
    }

    /// <summary>
    /// Sync components after row changes
    /// </summary>
    public void SyncComponents()
    {
        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>(_rowPositions));
    }

    /// <summary>
    /// Clear all row positions
    /// </summary>
    public void Clear()
    {
        _rowPositions.Clear();
        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>());
    }

    /// <summary>
    /// Handle tree expand/collapse operations
    /// </summary>
    public void HandleTreeOperation(int parentIndex, bool expanded)
    {
        if (!_rowPositions.ContainsKey(parentIndex))
            return;

        _rowPositions[parentIndex] = _rowPositions[parentIndex] with { Expanded = expanded };

        // Recalculate positions for all rows below the parent
        RecalculatePositionsFromIndex(parentIndex + 1);
    }

    private void RecalculateAllPositions()
    {
        if (!_rowPositions.Any()) return;

        var currentTop = _headerHeight;
        foreach (var kvp in _rowPositions.OrderBy(x => x.Key))
        {
            var metrics = kvp.Value;
            _rowPositions[kvp.Key] = metrics with
            {
                Top = currentTop,
                Height = _defaultRowHeight
            };
            currentTop += _defaultRowHeight;
        }

        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>(_rowPositions));
    }

    private void RecalculatePositionsFromIndex(int startIndex)
    {
        var sortedRows = _rowPositions.OrderBy(x => x.Key).ToList();
        var startRow = sortedRows.FirstOrDefault(x => x.Key >= startIndex);

        if (startRow.Key == 0 && startRow.Value == null) return;

        var currentTop = startIndex > 0
            ? _rowPositions[startIndex - 1].Top + _rowPositions[startIndex - 1].Height
            : _headerHeight;

        foreach (var kvp in sortedRows.Where(x => x.Key >= startIndex))
        {
            var metrics = kvp.Value;
            _rowPositions[kvp.Key] = metrics with { Top = currentTop };
            currentTop += metrics.Height;
        }

        RowPositionsChanged?.Invoke(new Dictionary<int, RowMetrics>(_rowPositions));
    }

    private static int GetTaskId(object item)
    {
        // Use reflection to get TaskId property
        var property = item.GetType().GetProperty("Id") ?? item.GetType().GetProperty("TaskId");
        return property?.GetValue(item) as int? ?? 0;
    }
}

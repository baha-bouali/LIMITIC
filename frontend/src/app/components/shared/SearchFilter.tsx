import { Search, Filter, X } from 'lucide-react';
import { useState } from 'react';
import { Badge } from '../ui/Badge';
import { clsx } from 'clsx';

interface FilterOption {
  id: string;
  label: string;
  value: string;
}

interface FilterGroup {
  id: string;
  label: string;
  options: FilterOption[];
  multi?: boolean;
}

interface SearchFilterProps {
  searchPlaceholder?: string;
  filterGroups?: FilterGroup[];
  onSearchChange?: (value: string) => void;
  onFilterChange?: (filters: Record<string, string | string[]>) => void;
  initialSearch?: string;
  initialFilters?: Record<string, string | string[]>;
}

export function SearchFilter({
  searchPlaceholder = 'Rechercher...',
  filterGroups = [],
  onSearchChange,
  onFilterChange,
  initialSearch = '',
  initialFilters = {}
}: SearchFilterProps) {
  const [search, setSearch] = useState(initialSearch);
  const [filters, setFilters] = useState<Record<string, string | string[]>>(initialFilters);
  const [showFilters, setShowFilters] = useState(false);

  const handleSearchChange = (value: string) => {
    setSearch(value);
    onSearchChange?.(value);
  };

  const handleFilterChange = (groupId: string, value: string, multi: boolean = false) => {
    const newFilters = { ...filters };

    if (multi) {
      const currentValues = (newFilters[groupId] as string[]) || [];
      if (currentValues.includes(value)) {
        newFilters[groupId] = currentValues.filter(v => v !== value);
      } else {
        newFilters[groupId] = [...currentValues, value];
      }
    } else {
      newFilters[groupId] = value;
    }

    setFilters(newFilters);
    onFilterChange?.(newFilters);
  };

  const clearFilter = (groupId: string) => {
    const newFilters = { ...filters };
    delete newFilters[groupId];
    setFilters(newFilters);
    onFilterChange?.(newFilters);
  };

  const clearAllFilters = () => {
    setFilters({});
    onFilterChange?.({});
  };

  const activeFilterCount = Object.keys(filters).filter(key => {
    const value = filters[key];
    return Array.isArray(value) ? value.length > 0 : value !== '';
  }).length;

  return (
    <div className="space-y-4">
      <div className="flex gap-3">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-text-muted" size={20} />
          <input
            type="text"
            value={search}
            onChange={(e) => handleSearchChange(e.target.value)}
            placeholder={searchPlaceholder}
            className="w-full pl-10 pr-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
          />
        </div>
        <button
          onClick={() => setShowFilters(!showFilters)}
          className={clsx(
            'flex items-center gap-2 px-4 py-3 border border-surface-border rounded-lg transition-colors',
            showFilters ? 'bg-accent-blue text-white border-accent-blue' : 'hover:bg-light-gray dark:hover:bg-input-background'
          )}
        >
          <Filter size={20} />
          <span className="hidden md:inline">Filtres</span>
          {activeFilterCount > 0 && (
            <Badge variant="default" className="!bg-white !text-accent-blue !px-2 !py-0.5">
              {activeFilterCount}
            </Badge>
          )}
        </button>
      </div>

      {showFilters && filterGroups.length > 0 && (
        <div className="p-6 bg-light-gray dark:bg-card rounded-lg border border-surface-border">
          <div className="flex items-center justify-between mb-4">
            <h4 className="font-semibold text-navy dark:text-white">Filtres</h4>
            {activeFilterCount > 0 && (
              <button
                onClick={clearAllFilters}
                className="text-sm text-accent-blue hover:underline"
              >
                Tout effacer
              </button>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filterGroups.map((group) => (
              <div key={group.id}>
                <div className="flex items-center justify-between mb-2">
                  <label className="text-sm font-medium text-navy dark:text-white">
                    {group.label}
                  </label>
                  {filters[group.id] && (
                    <button
                      onClick={() => clearFilter(group.id)}
                      className="text-xs text-text-muted hover:text-accent-blue"
                    >
                      <X size={14} />
                    </button>
                  )}
                </div>

                {group.multi ? (
                  <div className="space-y-2">
                    {group.options.map((option) => {
                      const isSelected = (filters[group.id] as string[] || []).includes(option.value);
                      return (
                        <label key={option.id} className="flex items-center gap-2 cursor-pointer">
                          <input
                            type="checkbox"
                            checked={isSelected}
                            onChange={() => handleFilterChange(group.id, option.value, true)}
                            className="w-4 h-4 text-accent-blue border-surface-border rounded focus:ring-accent-blue"
                          />
                          <span className="text-sm">{option.label}</span>
                        </label>
                      );
                    })}
                  </div>
                ) : (
                  <select
                    value={(filters[group.id] as string) || ''}
                    onChange={(e) => handleFilterChange(group.id, e.target.value)}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  >
                    <option value="">Tous</option>
                    {group.options.map((option) => (
                      <option key={option.id} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                )}
              </div>
            ))}
          </div>

          {activeFilterCount > 0 && (
            <div className="mt-4 pt-4 border-t border-surface-border">
              <div className="text-xs text-text-muted mb-2">Filtres actifs:</div>
              <div className="flex flex-wrap gap-2">
                {Object.entries(filters).map(([key, value]) => {
                  const group = filterGroups.find(g => g.id === key);
                  if (!group) return null;

                  const values = Array.isArray(value) ? value : [value];
                  return values.filter(v => v).map((v) => {
                    const option = group.options.find(o => o.value === v);
                    if (!option) return null;

                    return (
                      <Badge
                        key={`${key}-${v}`}
                        variant="info"
                        className="flex items-center gap-2 cursor-pointer"
                        onClick={() => {
                          if (Array.isArray(value)) {
                            handleFilterChange(key, v, true);
                          } else {
                            clearFilter(key);
                          }
                        }}
                      >
                        {option.label}
                        <X size={12} />
                      </Badge>
                    );
                  });
                })}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

import { useState, useRef, useEffect } from 'react';
import { toast } from 'sonner';
import limticLogo from '@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview.png';
import iconLogo from '@/imports/icon.png';
import { Navigate, Outlet, Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useLanguage } from '../../contexts/LanguageContext';
import {
  Menu, Bell, LogOut, Home, Users, FileText, Calendar, Target, BarChart3,
  Shield, Settings, BookOpen, User, FolderOpen, Sun, Moon, ChevronLeft, ChevronDown
} from 'lucide-react';
import { Badge } from '../../components/ui/Badge';
import { LanguageSwitcher } from '../../components/shared/LanguageSwitcher';
import { clsx } from 'clsx';
import { useTheme } from '../../contexts/ThemeContext';
import { getDashboardSlugForRole } from '../../auth/session';

export default function DashboardLayout() {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const [showNotifications, setShowNotifications] = useState(false);
  // Must be declared here — hooks cannot appear after conditional returns
  const [logoutLoading, setLogoutLoading] = useState(false);
  const profileMenuRef = useRef<HTMLDivElement>(null);
  const notifMenuRef = useRef<HTMLDivElement>(null);
  const { user, logout, isReady } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const { t } = useLanguage();
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (profileMenuRef.current && !profileMenuRef.current.contains(event.target as Node)) {
        setShowProfileMenu(false);
      }
      if (notifMenuRef.current && !notifMenuRef.current.contains(event.target as Node)) {
        setShowNotifications(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  if (!isReady) {
    return null;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  const handleLogout = async () => {
    setShowProfileMenu(false);
    setLogoutLoading(true);
    try {
      await logout();
      toast.success(t('dash.logoutSuccess') ?? 'Logged out');
      navigate('/login');
    } catch (err) {
      toast.error(t('dash.logoutError') ?? 'Logout failed');
    } finally {
      setLogoutLoading(false);
    }
  };

  const getRoleNavItems = () => {
    switch (user.role) {
      case 'SUPER_ADMIN':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/superadmin' },
          { icon: Users, label: t('dash.users'), href: '/dashboard/superadmin/users' },
          { icon: FileText, label: t('nav.publications'), href: '/dashboard/superadmin/publications' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/superadmin/events' },
          { icon: Target, label: t('dash.researchAxes'), href: '/dashboard/superadmin/axes' },
          { icon: BarChart3, label: t('dash.statistics'), href: '/dashboard/superadmin/statistics' },
          { icon: Shield, label: t('dash.audit'), href: '/dashboard/superadmin/audit' },
          { icon: Bell, label: t('dash.notifications'), href: '/dashboard/superadmin/notifications' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/superadmin/profile' },
          { icon: Settings, label: t('dash.settings'), href: '/dashboard/superadmin/settings' },
        ];
      case 'ADMIN':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/admin' },
          { icon: Users, label: t('dash.users'), href: '/dashboard/admin/users' },
          { icon: FileText, label: t('nav.publications'), href: '/dashboard/admin/publications' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/admin/events' },
          { icon: Target, label: t('dash.researchAxes'), href: '/dashboard/admin/axes' },
          { icon: BarChart3, label: t('dash.statistics'), href: '/dashboard/admin/statistics' },
          { icon: Bell, label: t('dash.notifications'), href: '/dashboard/admin/notifications' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/admin/profile' },
        ];
      case 'CHERCHEUR':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/chercheur' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/chercheur/profile' },
          { icon: FileText, label: t('pub.myPublications'), href: '/dashboard/chercheur/publications' },
          { icon: FolderOpen, label: t('dash.supervisions'), href: '/dashboard/chercheur/encadrements' },
          { icon: Target, label: t('dash.axes'), href: '/dashboard/chercheur/axes' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/chercheur/events' },
          { icon: Users, label: t('team.labTeam'), href: '/dashboard/chercheur/team' },
          { icon: Bell, label: t('dash.notifications'), href: '/dashboard/chercheur/notifications' },
        ];
      case 'DOCTORANT':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/doctorant' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/doctorant/profile' },
          { icon: FileText, label: t('pub.myPublications'), href: '/dashboard/doctorant/publications' },
          { icon: BookOpen, label: t('dash.allPublications'), href: '/dashboard/doctorant/all-publications' },
          { icon: Target, label: t('dash.axes'), href: '/dashboard/doctorant/axes' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/doctorant/events' },
          { icon: Users, label: t('dash.team'), href: '/dashboard/doctorant/team' },
          { icon: Bell, label: t('dash.notifications'), href: '/dashboard/doctorant/notifications' },
        ];
      case 'MASTERIEN':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/masterien' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/masterien/profile' },
          { icon: BookOpen, label: t('dash.allPublications'), href: '/dashboard/masterien/all-publications' },
          { icon: Target, label: t('dash.axes'), href: '/dashboard/masterien/axes' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/masterien/events' },
          { icon: Users, label: t('dash.team'), href: '/dashboard/masterien/team' },
          { icon: Bell, label: t('dash.notifications'), href: '/dashboard/masterien/notifications' },
        ];
      case 'VISITOR':
        return [
          { icon: Home, label: t('dash.dashboard'), href: '/dashboard/visitor' },
          { icon: User, label: t('dash.profile'), href: '/dashboard/visitor/profile' },
          { icon: BookOpen, label: t('dash.allPublications'), href: '/dashboard/visitor/all-publications' },
          { icon: Target, label: t('dash.axes'), href: '/dashboard/visitor/axes' },
          { icon: Calendar, label: t('dash.events'), href: '/dashboard/visitor/events' },
          { icon: Users, label: t('dash.team'), href: '/dashboard/visitor/team' },
        ];
      default:
        return [];
    }
  };

  const navItems = getRoleNavItems();

  const getRoleBadgeVariant = (role: string) => {
    switch (role) {
      case 'SUPER_ADMIN': return 'default';
      case 'ADMIN': return 'info';
      case 'CHERCHEUR': return 'success';
      case 'DOCTORANT': return 'warning';
      case 'MASTERIEN': return 'default';
      case 'VISITOR': return 'default';
      default: return 'default';
    }
  };

  const getRoleLabel = (role: string) => {
    switch (role) {
      case 'SUPER_ADMIN': return 'SuperAdmin';
      case 'ADMIN': return 'Admin';
      case 'CHERCHEUR': return 'Chercheur';
      case 'DOCTORANT': return 'Doctorant';
      case 'MASTERIEN': return 'Mastérien';
      case 'VISITOR': return 'Visiteur';
      default: return role;
    }
  };

  const sidebarDarkGradient = 'linear-gradient(180deg, #0d3318 0%, #0f1f3a 100%)';

  return (
    <div className="min-h-screen bg-light-gray dark:bg-background flex">
      {/* Sidebar */}
      <aside
        className={clsx(
          'fixed left-0 top-0 h-full text-white transition-all duration-300 z-50 flex flex-col',
          sidebarCollapsed ? 'w-[var(--sidebar-collapsed-width)]' : 'w-[var(--sidebar-width)]'
        )}
        style={{ background: theme === 'dark' ? sidebarDarkGradient : 'var(--brand-gradient)' }}
      >
        {/* Logo */}
        <div className="h-[var(--navbar-height)] flex items-center px-4 border-b border-white/10 flex-shrink-0">
          <div className={clsx('flex items-center gap-3', sidebarCollapsed && 'justify-center w-full')}>
            <div className="flex items-center justify-center flex-shrink-0">
              <img
                src={sidebarCollapsed ? iconLogo : limticLogo}
                alt="LIMTIC"
                className={sidebarCollapsed ? "w-8 h-8 object-contain" : "w-28 h-auto object-contain"}
                onError={(e) => {
                  const parent = e.currentTarget.parentElement;
                  if (parent) {
                    e.currentTarget.style.display = 'none';
                    parent.innerHTML = '<span style="font-weight:700;font-size:14px;color:white">L</span>';
                  }
                }}
              />
            </div>
            {!sidebarCollapsed && (
              <div className="flex-1 min-w-0">
                <div className="font-bold text-sm leading-tight">LIMTIC</div>
                <div className="text-xs opacity-50">{t('lab.shortName')}</div>
              </div>
            )}
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-3">
          <ul className="space-y-0.5 px-2">
            {navItems.map((item) => {
              const Icon = item.icon;
              const isActive = location.pathname === item.href ||
                (item.href !== '/dashboard/chercheur' &&
                 item.href !== '/dashboard/doctorant' &&
                 item.href !== '/dashboard/masterien' &&
                 item.href !== '/dashboard/admin' &&
                 item.href !== '/dashboard/superadmin' &&
                 location.pathname.startsWith(item.href + '/'));
              const tooltipBg = theme === 'dark' ? '#0d1e10' : '#0f2840';
              return (
                <li key={item.href}>
                  <Link
                    to={item.href}
                    title={sidebarCollapsed ? item.label : undefined}
                    className={clsx(
                      'flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors relative group',
                      isActive
                        ? 'bg-white/20 text-white'
                        : 'text-white/70 hover:bg-white/10 hover:text-white',
                      sidebarCollapsed && 'justify-center px-2'
                    )}
                  >
                    {isActive && <div className="absolute left-0 top-1/2 -translate-y-1/2 w-[3px] h-5 bg-white rounded-r-full" />}
                    <Icon size={17} className="flex-shrink-0" />
                    {!sidebarCollapsed && <span className="text-sm font-medium">{item.label}</span>}
                    {sidebarCollapsed && (
                      <div
                        className="absolute left-full ml-2 px-2.5 py-1.5 text-white text-xs rounded-lg opacity-0 group-hover:opacity-100 pointer-events-none whitespace-nowrap z-50 transition-opacity shadow-lg border border-white/10"
                        style={{ background: tooltipBg }}
                      >
                        {item.label}
                      </div>
                    )}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

        {/* Footer */}
        <div className="p-3 border-t border-white/10 flex-shrink-0 space-y-2">
          <button
            onClick={handleLogout}
            title={sidebarCollapsed ? t('dash.logout') : undefined}
            className={clsx(
              'flex items-center gap-3 px-3 py-2.5 rounded-lg text-white/60 hover:bg-white/10 hover:text-white transition-colors w-full group relative',
              sidebarCollapsed && 'justify-center px-2'
            )}
          >
            <LogOut size={17} />
            {!sidebarCollapsed && <span className="text-sm font-medium">{t('dash.logout')}</span>}
            {sidebarCollapsed && (
              <div
                className="absolute left-full ml-2 px-2.5 py-1.5 text-white text-xs rounded-lg opacity-0 group-hover:opacity-100 pointer-events-none whitespace-nowrap z-50 transition-opacity shadow-lg border border-white/10"
                style={{ background: theme === 'dark' ? '#0d1e10' : '#0f2840' }}
              >
                {t('dash.logout')}
              </div>
            )}
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div
        className={clsx(
          'flex-1 transition-all duration-300 min-w-0',
          sidebarCollapsed ? 'ml-[var(--sidebar-collapsed-width)]' : 'ml-[var(--sidebar-width)]'
        )}
      >
        {/* Top Bar */}
        <header className="h-[var(--navbar-height)] bg-white dark:bg-[#141c24] border-b border-surface-border dark:border-[#2d3d4e] sticky top-0 z-40 flex items-center justify-between px-5 gap-3">
          <button
            onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
            className="p-2 hover:bg-light-gray dark:hover:bg-[#1e2a35] rounded-lg transition-colors text-text-secondary dark:text-text-secondary hover:text-navy dark:hover:text-white"
            aria-label="Toggle sidebar"
          >
            {sidebarCollapsed ? <Menu size={20} /> : <ChevronLeft size={20} />}
          </button>

          <div className="flex items-center gap-2">
            {/* Language Switcher */}
            <LanguageSwitcher variant="navbar" className="hidden sm:flex" />

            {/* Theme Toggle */}
            <button
              onClick={toggleTheme}
              className="p-2 hover:bg-light-gray dark:hover:bg-[#1e2a35] rounded-lg transition-colors text-text-secondary dark:text-text-secondary hover:text-navy dark:hover:text-white"
              aria-label="Toggle theme"
            >
              {theme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
            </button>

            {/* Notifications */}
            <div className="relative" ref={notifMenuRef}>
              <button
                onClick={() => {
                  setShowNotifications(!showNotifications);
                  setShowProfileMenu(false);
                }}
                className="p-2 hover:bg-light-gray dark:hover:bg-[#1e2a35] rounded-lg transition-colors text-text-secondary dark:text-text-secondary hover:text-navy dark:hover:text-white relative"
                aria-label="Notifications"
              >
                <Bell size={18} />
                <span className="absolute top-1 right-1 w-2 h-2 bg-error rounded-full" />
              </button>

              {showNotifications && (
                <div className="absolute right-0 mt-2 w-80 bg-white dark:bg-[#141c24] border border-surface-border dark:border-[#2d3d4e] rounded-xl shadow-lg overflow-hidden">
                  <div className="p-4 border-b border-surface-border dark:border-[#2d3d4e]">
                    <h3 className="font-bold text-navy dark:text-white">{t('dash.notifications')}</h3>
                  </div>
                  <div className="max-h-96 overflow-y-auto">
                    {[
                      { id: 1, text: t('notif.pubApproved'), time: t('notif.ago2h'), unread: true },
                      { id: 2, text: t('notif.eventTomorrow'), time: t('notif.ago5h'), unread: true },
                      { id: 3, text: t('notif.profileUpdated'), time: t('notif.yesterday'), unread: false },
                    ].map(notif => (
                      <div
                        key={notif.id}
                        className={clsx(
                          'p-4 border-b border-surface-border dark:border-[#2d3d4e] hover:bg-light-gray dark:hover:bg-[#1e2a35] transition-colors cursor-pointer',
                          notif.unread && 'bg-accent-blue/5 dark:bg-accent-blue/10'
                        )}
                      >
                        <p className="text-sm font-medium text-navy dark:text-white">{notif.text}</p>
                        <p className="text-xs text-text-muted dark:text-text-muted mt-1">{notif.time}</p>
                      </div>
                    ))}
                  </div>
                  <div className="p-3 border-t border-surface-border dark:border-[#2d3d4e] text-center">
                    <Link
                      to={`/dashboard/${getDashboardSlugForRole(user.role)}/notifications`}
                      className="text-sm text-accent-blue hover:underline"
                      onClick={() => setShowNotifications(false)}
                    >
                      {t('common.viewAll')} {t('dash.notifications').toLowerCase()}
                    </Link>
                  </div>
                </div>
              )}
            </div>

            {/* Profile Menu */}
            <div className="relative" ref={profileMenuRef}>
              <button
                onClick={() => {
                  setShowProfileMenu(!showProfileMenu);
                  setShowNotifications(false);
                }}
                className="flex items-center gap-2 px-3 py-2 hover:bg-light-gray dark:hover:bg-[#1e2a35] rounded-lg transition-colors"
              >
                <div className="w-8 h-8 rounded-full flex items-center justify-center text-white text-xs font-bold" style={{ background: 'var(--brand-gradient)' }}>
                  {user.firstName.charAt(0)}{user.lastName.charAt(0)}
                </div>
                <div className="hidden md:block text-left">
                  <div className="text-xs text-text-muted dark:text-text-muted">
                    {new Date().getHours() < 12 ? t('greet.morning') : new Date().getHours() < 18 ? t('greet.afternoon') : t('greet.evening')}
                  </div>
                  <div className="text-sm font-medium text-navy dark:text-white">
                    {user.firstName} {user.lastName}
                  </div>
                </div>
                <ChevronDown size={16} className={clsx('text-text-muted dark:text-text-muted transition-transform', showProfileMenu && 'rotate-180')} />
              </button>

              {showProfileMenu && (
                <div className="absolute right-0 mt-2 w-64 bg-white dark:bg-[#141c24] border border-surface-border dark:border-[#2d3d4e] rounded-xl shadow-lg overflow-hidden">
                  <div className="p-4 border-b border-surface-border dark:border-[#2d3d4e]">
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 rounded-full flex items-center justify-center text-white font-bold" style={{ background: 'var(--brand-gradient)' }}>
                        {user.firstName.charAt(0)}{user.lastName.charAt(0)}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="font-medium text-navy dark:text-white truncate">
                          {user.firstName} {user.lastName}
                        </div>
                        <div className="text-xs text-text-muted dark:text-text-muted truncate">{user.email}</div>
                        <Badge variant={getRoleBadgeVariant(user.role)} className="mt-1 text-xs">
                          {getRoleLabel(user.role)}
                        </Badge>
                      </div>
                    </div>
                  </div>
                  <div className="p-2">
                    <Link
                      to={`/dashboard/${getDashboardSlugForRole(user.role)}/profile`}
                      className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-light-gray dark:hover:bg-[#1e2a35] transition-colors text-navy dark:text-white"
                      onClick={() => setShowProfileMenu(false)}
                    >
                      <User size={16} />
                      <span className="text-sm">{t('dash.profile')}</span>
                    </Link>
                  </div>
                  <div className="p-2 border-t border-surface-border dark:border-[#2d3d4e]">
                    <button
                      onClick={handleLogout}
                      disabled={logoutLoading}
                      aria-busy={logoutLoading}
                      className={clsx(
                        'flex items-center gap-3 px-3 py-2 rounded-lg transition-colors w-full',
                        logoutLoading ? 'opacity-60 cursor-wait' : 'hover:bg-error/10 text-error'
                      )}
                    >
                      <LogOut size={16} />
                      <span className="text-sm font-medium">{t('dash.logout')}</span>
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="p-6 lg:p-8">
          <div className="max-w-[1400px] mx-auto">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}

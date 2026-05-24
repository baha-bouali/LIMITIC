import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import { Toaster } from 'sonner';

// Public pages
import HomePage from './pages/public/HomePage';
import TeamPage from './pages/public/TeamPage';
import ResearcherPage from './pages/public/ResearcherPage';
import PublicationsPage from './pages/public/PublicationsPage';
import PublicationDetailPage from './pages/public/PublicationDetailPage';
import EventsPage from './pages/public/EventsPage';
import EventDetailPage from './pages/public/EventDetailPage';
import ContactPage from './pages/public/ContactPage';
import AxesPage from './pages/public/AxesPage';
import AxisDetailPage from './pages/public/AxisDetailPage';
import LoginPage from './pages/auth/LoginPage';
import ForgotPasswordPage from './pages/auth/ForgotPasswordPage';

// Dashboard layout
import DashboardLayout from './pages/dashboard/DashboardLayout';

// SuperAdmin pages
import SuperAdminOverview from './pages/dashboard/superadmin/Overview';
import SuperAdminUsers from './pages/dashboard/superadmin/Users';
import SuperAdminPublications from './pages/dashboard/superadmin/Publications';
import SuperAdminEvents from './pages/dashboard/superadmin/Events';
import SuperAdminAxes from './pages/dashboard/superadmin/Axes';
import SuperAdminAudit from './pages/dashboard/superadmin/Audit';
import SuperAdminSettings from './pages/dashboard/superadmin/Settings';
import SuperAdminStatistics from './pages/dashboard/superadmin/Statistics';
import SuperAdminNotifications from './pages/dashboard/superadmin/Notifications';
import SuperAdminProfile from './pages/dashboard/superadmin/Profile';

// Admin pages
import AdminOverview from './pages/dashboard/admin/Overview';
import AdminProfile from './pages/dashboard/admin/Profile';
import AdminNotifications from './pages/dashboard/admin/Notifications';
import AdminEvents from './pages/dashboard/admin/Events';

// Chercheur pages
import ChercheurOverview from './pages/dashboard/chercheur/Overview';
import ChercheurProfile from './pages/dashboard/chercheur/Profile';
import ChercheurPublications from './pages/dashboard/chercheur/Publications';
import ChercheurEncadrements from './pages/dashboard/chercheur/Encadrements';
import ChercheurNotifications from './pages/dashboard/chercheur/Notifications';
import ChercheurEvents from './pages/dashboard/chercheur/Events';
import ChercheurTeam from './pages/dashboard/chercheur/Team';
import ChercheurAxes from './pages/dashboard/chercheur/Axes';

// Doctorant pages
import DoctorantOverview from './pages/dashboard/doctorant/Overview';
import DoctorantProfile from './pages/dashboard/doctorant/Profile';
import DoctorantPublications from './pages/dashboard/doctorant/Publications';
import DoctorantAllPublications from './pages/dashboard/doctorant/AllPublications';
import DoctorantTeam from './pages/dashboard/doctorant/Team';
import DoctorantNotifications from './pages/dashboard/doctorant/Notifications';
import DoctorantEvents from './pages/dashboard/doctorant/Events';
import DoctorantAxes from './pages/dashboard/doctorant/Axes';

// Mastérien pages
import MasterienOverview from './pages/dashboard/masterien/Overview';
import MasterienProfile from './pages/dashboard/masterien/Profile';
import MasterienPublications from './pages/dashboard/masterien/Publications';
import MasterienAllPublications from './pages/dashboard/masterien/AllPublications';
import MasterienTeam from './pages/dashboard/masterien/Team';
import MasterienNotifications from './pages/dashboard/masterien/Notifications';
import MasterienEvents from './pages/dashboard/masterien/Events';
import MasterienAxes from './pages/dashboard/masterien/Axes';

// Visitor pages
import VisitorOverview from './pages/dashboard/visitor/Overview';
import VisitorProfile from './pages/dashboard/visitor/Profile';
import VisitorAllPublications from './pages/dashboard/visitor/AllPublications';
import VisitorTeam from './pages/dashboard/visitor/Team';
import VisitorEvents from './pages/dashboard/visitor/Events';
import VisitorAxes from './pages/dashboard/visitor/Axes';

// Contexts
import { AuthProvider } from './contexts/AuthContext';
import { ThemeProvider } from './contexts/ThemeContext';
import { LanguageProvider } from './contexts/LanguageContext';
import { useAuth } from './contexts/AuthContext';
import { getDashboardPathForRole, type UserRole } from './auth/session';

const DASHBOARD_ACCESS: Record<string, UserRole[]> = {
  superadmin: ['SUPER_ADMIN'],
  admin: ['SUPER_ADMIN', 'ADMIN'],
  chercheur: ['SUPER_ADMIN', 'CHERCHEUR'],
  doctorant: ['SUPER_ADMIN', 'DOCTORANT'],
  masterien: ['SUPER_ADMIN', 'MASTERIEN'],
  visitor: ['SUPER_ADMIN', 'VISITOR'],
};

function DashboardRoleGuard({ allowedRoles }: { allowedRoles: UserRole[] }) {
  const { user, role, isReady } = useAuth();

  if (!isReady) return null;
  if (!user) return <Navigate to="/login" replace />;

  const currentRole = role ?? user.role;
  if (!allowedRoles.includes(currentRole)) {
    return <Navigate to={getDashboardPathForRole(currentRole)} replace />;
  }

  return <Outlet />;
}

function DashboardIndexRedirect() {
  const { role, isAuthenticated, isReady } = useAuth();

  if (!isReady) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Navigate to={getDashboardPathForRole(role)} replace />;
}

export default function App() {
  return (
    <ThemeProvider>
      <LanguageProvider>
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              {/* Public routes */}
              <Route path="/" element={<HomePage />} />
              <Route path="/equipe" element={<TeamPage />} />
              <Route path="/chercheurs/:id" element={<ResearcherPage />} />
              <Route path="/publications" element={<PublicationsPage />} />
              <Route path="/publications/:id" element={<PublicationDetailPage />} />
              <Route path="/axes-recherche" element={<AxesPage />} />
              <Route path="/axes-recherche/:id" element={<AxisDetailPage />} />
              <Route path="/evenements" element={<EventsPage />} />
              <Route path="/evenements/:id" element={<EventDetailPage />} />
              <Route path="/contact" element={<ContactPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/forgot-password" element={<ForgotPasswordPage />} />

              {/* Dashboard routes */}
              <Route path="/dashboard" element={<DashboardLayout />}>
                {/* Default redirect */}
                <Route index element={<DashboardIndexRedirect />} />

                {/* SuperAdmin routes */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.superadmin} />}>
                  <Route path="superadmin" element={<SuperAdminOverview />} />
                  <Route path="superadmin/users" element={<SuperAdminUsers />} />
                  <Route path="superadmin/publications" element={<SuperAdminPublications />} />
                  <Route path="superadmin/events" element={<SuperAdminEvents />} />
                  <Route path="superadmin/axes" element={<SuperAdminAxes />} />
                  <Route path="superadmin/audit" element={<SuperAdminAudit />} />
                  <Route path="superadmin/settings" element={<SuperAdminSettings />} />
                  <Route path="superadmin/statistics" element={<SuperAdminStatistics />} />
                  <Route path="superadmin/notifications" element={<SuperAdminNotifications />} />
                  <Route path="superadmin/profile" element={<SuperAdminProfile />} />
                </Route>

                {/* Admin routes */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.admin} />}>
                  <Route path="admin" element={<AdminOverview />} />
                  <Route path="admin/users" element={<SuperAdminUsers />} />
                  <Route path="admin/publications" element={<SuperAdminPublications />} />
                  <Route path="admin/events" element={<AdminEvents />} />
                  <Route path="admin/axes" element={<SuperAdminAxes />} />
                  <Route path="admin/statistics" element={<SuperAdminStatistics />} />
                  <Route path="admin/notifications" element={<AdminNotifications />} />
                  <Route path="admin/profile" element={<AdminProfile />} />
                </Route>

                {/* Chercheur routes */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.chercheur} />}>
                  <Route path="chercheur" element={<ChercheurOverview />} />
                  <Route path="chercheur/profile" element={<ChercheurProfile />} />
                  <Route path="chercheur/publications" element={<ChercheurPublications />} />
                  <Route path="chercheur/encadrements" element={<ChercheurEncadrements />} />
                  <Route path="chercheur/axes" element={<ChercheurAxes />} />
                  <Route path="chercheur/notifications" element={<ChercheurNotifications />} />
                  <Route path="chercheur/events" element={<ChercheurEvents />} />
                  <Route path="chercheur/team" element={<ChercheurTeam />} />
                </Route>

                {/* Doctorant routes */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.doctorant} />}>
                  <Route path="doctorant" element={<DoctorantOverview />} />
                  <Route path="doctorant/profile" element={<DoctorantProfile />} />
                  <Route path="doctorant/publications" element={<DoctorantPublications />} />
                  <Route path="doctorant/all-publications" element={<DoctorantAllPublications />} />
                  <Route path="doctorant/axes" element={<DoctorantAxes />} />
                  <Route path="doctorant/team" element={<DoctorantTeam />} />
                  <Route path="doctorant/notifications" element={<DoctorantNotifications />} />
                  <Route path="doctorant/events" element={<DoctorantEvents />} />
                </Route>

                {/* Mastérien routes */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.masterien} />}>
                  <Route path="masterien" element={<MasterienOverview />} />
                  <Route path="masterien/profile" element={<MasterienProfile />} />
                  <Route path="masterien/all-publications" element={<MasterienAllPublications />} />
                  <Route path="masterien/axes" element={<MasterienAxes />} />
                  <Route path="masterien/team" element={<MasterienTeam />} />
                  <Route path="masterien/notifications" element={<MasterienNotifications />} />
                  <Route path="masterien/events" element={<MasterienEvents />} />
                </Route>

                {/* Visitor routes - reuse Masterien pages since same permissions */}
                <Route element={<DashboardRoleGuard allowedRoles={DASHBOARD_ACCESS.visitor} />}>
                  <Route path="visitor" element={<VisitorOverview />} />
                  <Route path="visitor/profile" element={<VisitorProfile />} />
                  <Route path="visitor/all-publications" element={<VisitorAllPublications />} />
                  <Route path="visitor/axes" element={<VisitorAxes />} />
                  <Route path="visitor/team" element={<VisitorTeam />} />
                  <Route path="visitor/events" element={<VisitorEvents />} />
                </Route>
              </Route>

              {/* 404 fallback */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
            <Toaster position="top-right" richColors />
          </BrowserRouter>
        </AuthProvider>
      </LanguageProvider>
    </ThemeProvider>
  );
}

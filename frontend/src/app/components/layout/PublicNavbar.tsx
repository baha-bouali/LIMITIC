import image_5b435523_dd99_4ba8_9226_dcd9ab960a41_removebg_preview_2 from "@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview-2.png";
import image_5b435523_dd99_4ba8_9226_dcd9ab960a41_removebg_preview_1 from "@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview-1.png";
import { useState, useEffect } from "react";
import limticLogo from "@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview.png";
import { Link, useLocation } from "react-router-dom";
import { Menu, X, Moon, Sun } from "lucide-react";
import { Button } from "../ui/Button";
import { LanguageSwitcher } from "../shared/LanguageSwitcher";
import { useLanguage } from "../../contexts/LanguageContext";
import { useTheme } from "../../contexts/ThemeContext";
import { clsx } from "clsx";

export function PublicNavbar() {
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] =
    useState(false);
  const location = useLocation();
  const { t } = useLanguage();
  const { theme, toggleTheme } = useTheme();

  const navLinks = [
    { label: t("nav.home"), href: "/" },
    { label: t("nav.team"), href: "/equipe" },
    { label: t("nav.publications"), href: "/publications" },
    { label: t("nav.axes"), href: "/axes-recherche" },
    { label: t("nav.events"), href: "/evenements" },
    { label: t("nav.contact"), href: "/contact" },
  ];

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 20);
    };
    window.addEventListener("scroll", handleScroll);
    return () =>
      window.removeEventListener("scroll", handleScroll);
  }, []);

  const isHomePage = location.pathname === "/";
  const isTransparent = isHomePage && !isScrolled;

  return (
    <nav
      className={clsx(
        "fixed top-0 left-0 right-0 z-50 transition-all duration-300",
        isTransparent
          ? "bg-transparent"
          : "bg-white dark:bg-[#141c24] shadow-md",
      )}
      style={{ height: "var(--navbar-height)" }}
    >
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 h-full flex items-center justify-between">
        {/* Logo */}
        <Link to="/" className="flex items-center gap-3">
          <div className="flex items-center justify-center w-16 h-16 overflow-hidden">
            <img
              src={
                image_5b435523_dd99_4ba8_9226_dcd9ab960a41_removebg_preview_2
              }
              alt="LIMTIC"
              className="w-[1200px] h-20 object-contain rounded-t-[0px] rounded-bl-[61px] rounded-br-[0px] mx-[2000px] my-[0px]"
              onError={(e) => {
                const parent = e.currentTarget.parentElement;
                if (parent) {
                  e.currentTarget.style.display = "none";
                  parent.innerHTML = `<span class="${isTransparent ? "text-white" : "text-navy dark:text-white"} font-bold text-2xl">L</span>`;
                }
              }}
            />
          </div>
        </Link>

        {/* Desktop Navigation */}
        <div className="hidden md:flex items-center gap-8">
          {navLinks.map((link) => {
            const isActive = location.pathname === link.href;
            return (
              <Link
                key={link.href}
                to={link.href}
                className={clsx(
                  "relative py-2 transition-colors",
                  isTransparent
                    ? "text-white hover:text-white/80"
                    : "text-text-primary dark:text-text-primary hover:text-accent-blue dark:hover:text-accent-blue",
                  isActive && "font-medium",
                )}
              >
                {link.label}
                {isActive && (
                  <div
                    className={clsx(
                      "absolute bottom-0 left-0 right-0 h-0.5 rounded-full",
                      isTransparent
                        ? "bg-white"
                        : "bg-accent-blue",
                    )}
                  />
                )}
              </Link>
            );
          })}
        </div>

        {/* Right Actions */}
        <div className="hidden md:flex items-center gap-4">
          <button
            onClick={toggleTheme}
            className={clsx(
              "p-2 rounded-lg transition-colors",
              isTransparent
                ? "text-white hover:bg-white/10"
                : "text-text-primary dark:text-text-primary hover:bg-light-gray dark:hover:bg-[#1e2a35]",
            )}
            aria-label="Toggle theme"
          >
            {theme === "light" ? (
              <Moon size={20} />
            ) : (
              <Sun size={20} />
            )}
          </button>
          <LanguageSwitcher
            variant="navbar"
            className={clsx(
              isTransparent &&
                "!bg-white/10 [&_button]:!text-white [&_button:hover]:!bg-white/20 [&_button.bg-accent-blue]:!bg-white/30",
            )}
          />
          <Link to="/login">
            <Button
              variant={isTransparent ? "outlined" : "outlined"}
              className={
                isTransparent
                  ? "!text-white !border-white hover:!bg-white/10"
                  : ""
              }
            >
              {t("nav.login")}
            </Button>
          </Link>
        </div>

        {/* Mobile Menu Button */}
        <button
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
          className={clsx(
            "md:hidden p-2 rounded-lg",
            isTransparent
              ? "text-white"
              : "text-navy dark:text-white",
          )}
        >
          {isMobileMenuOpen ? (
            <X size={24} />
          ) : (
            <Menu size={24} />
          )}
        </button>
      </div>

      {/* Mobile Menu */}
      {isMobileMenuOpen && (
        <div className="md:hidden absolute top-full left-0 right-0 bg-white dark:bg-[#141c24] shadow-lg border-t border-surface-border dark:border-[#2d3d4e]">
          <div className="p-6 space-y-4">
            {navLinks.map((link) => (
              <Link
                key={link.href}
                to={link.href}
                onClick={() => setIsMobileMenuOpen(false)}
                className={clsx(
                  "block py-2 text-text-primary dark:text-text-primary hover:text-accent-blue dark:hover:text-accent-blue transition-colors",
                  location.pathname === link.href &&
                    "font-medium text-accent-blue",
                )}
              >
                {link.label}
              </Link>
            ))}
            <div className="pt-4 border-t border-surface-border dark:border-[#2d3d4e] space-y-3">
              <button
                onClick={toggleTheme}
                className="flex items-center gap-2 w-full px-4 py-2 rounded-lg hover:bg-light-gray dark:hover:bg-[#1e2a35] transition-colors text-text-primary dark:text-text-primary"
              >
                {theme === "light" ? (
                  <Moon size={18} />
                ) : (
                  <Sun size={18} />
                )}
                <span className="text-sm font-medium">
                  {theme === "light"
                    ? t("theme.dark")
                    : t("theme.light")}
                </span>
              </button>
              <div className="px-4">
                <LanguageSwitcher variant="navbar" />
              </div>
              <Link
                to="/login"
                onClick={() => setIsMobileMenuOpen(false)}
                className="block"
              >
                <Button variant="outlined" className="w-full">
                  {t("nav.login")}
                </Button>
              </Link>
            </div>
          </div>
        </div>
      )}
    </nav>
  );
}
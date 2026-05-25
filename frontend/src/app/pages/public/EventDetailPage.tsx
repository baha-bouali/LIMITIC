import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Calendar, MapPin, Users, Download, Mail, Briefcase, Image as ImageIcon } from 'lucide-react';
import { clsx } from 'clsx';
import { useLanguage } from '../../contexts/LanguageContext';
import { formatEventDate, useGetEventByIdQuery } from '../../api/eventsApi';
import jsPDF from 'jspdf';

export default function EventDetailPage() {
  const { id } = useParams();
  const { t } = useLanguage();
  const { data: event, isLoading, isError } = useGetEventByIdQuery(id ?? '', { skip: !id });

  const statusConfig = useMemo(
    () => ({
      A_VENIR: { variant: 'info' as const, label: t('eventPage.upcoming') },
      EN_COURS: { variant: 'success' as const, label: t('eventPage.ongoing') },
      PASSE: { variant: 'default' as const, label: t('eventPage.past') },
    }),
    [t],
  );

  const typeBadgeClass = (type: string) =>
    ({
      SÉMINAIRE: 'bg-[#EFF6FF] text-[#1D4ED8]',
      ATELIER: 'bg-[#F0FDF4] text-[#15803D]',
      CONFÉRENCE: 'bg-[#FFF7ED] text-[#C2410C]',
      "JOURNÉE D'ÉTUDE": 'bg-[#FAF5FF] text-[#7E22CE]',
    }[type] || 'bg-light-gray text-text-secondary');

  const speakers = event?.speakers ?? [];
  const photoFileNames = event?.photoFileNames ?? [];

  const formatDate = (dateValue?: string) => {
    return formatEventDate(dateValue, {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatDay = (dateValue?: string) => {
    return formatEventDate(dateValue, {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
    });
  };

  const getSpeakerInitials = (firstName: string, lastName: string) =>
    `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();

  // ─── PDF Download ──────────────────────────────────────────────────────────
  const handleDownloadProgram = () => {
    if (!event) return;

    const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

    const pageWidth = doc.internal.pageSize.getWidth();
    const pageHeight = doc.internal.pageSize.getHeight();
    const margin = 20;
    const contentWidth = pageWidth - margin * 2;
    let y = margin;

    const addPageIfNeeded = (neededHeight: number) => {
      if (y + neededHeight > pageHeight - margin) {
        doc.addPage();
        y = margin;
      }
    };

    // ── Header band ────────────────────────────────────────
    doc.setFillColor(15, 40, 80); // navy
    doc.rect(0, 0, pageWidth, 48, 'F');

    // Accent stripe
    doc.setFillColor(59, 130, 246); // accent-blue
    doc.rect(0, 45, pageWidth, 3, 'F');

    doc.setTextColor(255, 255, 255);
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(20);
    const titleLines = doc.splitTextToSize(event.title, contentWidth);
    titleLines.forEach((line: string, i: number) => {
      doc.text(line, margin, 18 + i * 9);
    });

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.setTextColor(180, 200, 240);
    const headerY = 18 + titleLines.length * 9 + 2;
    doc.text(`${event.type}  •  ${formatDay(event.startDate)}`, margin, Math.min(headerY, 38));
    doc.text(event.location ?? '', margin, Math.min(headerY + 6, 44));

    y = 58;

    // ── Meta row ───────────────────────────────────────────
    // Dates box
    doc.setFillColor(245, 248, 255);
    doc.roundedRect(margin, y, contentWidth / 2 - 4, 22, 3, 3, 'F');
    doc.setTextColor(15, 40, 80);
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(9);
    doc.text('DATES', margin + 5, y + 7);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(60, 60, 60);
    doc.setFontSize(9);
    const dateStr = `${formatDate(event.startDate)}`;
    const dateStr2 = `${formatDate(event.endDate)}`;
    doc.text(dateStr, margin + 5, y + 13, { maxWidth: contentWidth / 2 - 10 });
    doc.text(dateStr2, margin + 5, y + 18, { maxWidth: contentWidth / 2 - 10 });

    // Location box
    const lx = margin + contentWidth / 2 + 4;
    doc.setFillColor(245, 248, 255);
    doc.roundedRect(lx, y, contentWidth / 2 - 4, 22, 3, 3, 'F');
    doc.setTextColor(15, 40, 80);
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(9);
    doc.text('LIEU', lx + 5, y + 7);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(60, 60, 60);
    doc.setFontSize(9);
    doc.text(event.location ?? '—', lx + 5, y + 14, { maxWidth: contentWidth / 2 - 10 });

    y += 30;

    // ── Divider ────────────────────────────────────────────
    doc.setDrawColor(220, 225, 235);
    doc.setLineWidth(0.3);
    doc.line(margin, y, pageWidth - margin, y);
    y += 8;

    // ── Description ────────────────────────────────────────
    if (event.description) {
      addPageIfNeeded(12);
      doc.setFillColor(59, 130, 246);
      doc.rect(margin, y, 3, 7, 'F');
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(15, 40, 80);
      doc.setFontSize(13);
      doc.text('Description', margin + 7, y + 5.5);
      y += 12;

      doc.setFont('helvetica', 'normal');
      doc.setTextColor(70, 70, 70);
      doc.setFontSize(10);
      const descLines = doc.splitTextToSize(event.description, contentWidth);
      descLines.forEach((line: string) => {
        addPageIfNeeded(6);
        doc.text(line, margin, y);
        y += 5.5;
      });
      y += 6;
    }

    // ── Programme ──────────────────────────────────────────
    if (event.program) {
      addPageIfNeeded(14);
      doc.setDrawColor(220, 225, 235);
      doc.line(margin, y, pageWidth - margin, y);
      y += 8;

      doc.setFillColor(59, 130, 246);
      doc.rect(margin, y, 3, 7, 'F');
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(15, 40, 80);
      doc.setFontSize(13);
      doc.text('Programme scientifique', margin + 7, y + 5.5);
      y += 12;

      doc.setFont('helvetica', 'normal');
      doc.setTextColor(60, 70, 90);
      doc.setFontSize(10);
      const progLines = doc.splitTextToSize(event.program, contentWidth - 8);
      const blockHeight = progLines.length * 5.5 + 12;
      addPageIfNeeded(blockHeight);

      doc.setFillColor(239, 246, 255);
      doc.roundedRect(margin, y, contentWidth, blockHeight, 3, 3, 'F');
      doc.setFillColor(59, 130, 246);
      doc.rect(margin, y, 3, blockHeight, 'F');

      y += 7;
      progLines.forEach((line: string) => {
        addPageIfNeeded(6);
        doc.text(line, margin + 7, y);
        y += 5.5;
      });
      y += 8;
    }

    // ── Speakers ───────────────────────────────────────────
    if (speakers.length > 0) {
      addPageIfNeeded(16);
      doc.setDrawColor(220, 225, 235);
      doc.line(margin, y, pageWidth - margin, y);
      y += 8;

      doc.setFillColor(59, 130, 246);
      doc.rect(margin, y, 3, 7, 'F');
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(15, 40, 80);
      doc.setFontSize(13);
      doc.text('Intervenants', margin + 7, y + 5.5);
      y += 14;

      speakers.forEach((speaker, idx) => {
        const cardHeight = speaker.subject ? 36 : 30;
        addPageIfNeeded(cardHeight + 4);

        // Card background
        doc.setFillColor(249, 250, 252);
        doc.roundedRect(margin, y, contentWidth, cardHeight, 3, 3, 'F');
        doc.setDrawColor(220, 225, 235);
        doc.setLineWidth(0.2);
        doc.roundedRect(margin, y, contentWidth, cardHeight, 3, 3, 'S');

        // Avatar circle
        doc.setFillColor(15, 40, 80);
        doc.circle(margin + 12, y + cardHeight / 2, 9, 'F');
        doc.setTextColor(255, 255, 255);
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(9);
        doc.text(
          getSpeakerInitials(speaker.firstName, speaker.lastName),
          margin + 12,
          y + cardHeight / 2 + 3,
          { align: 'center' },
        );

        const sx = margin + 26;

        // Name
        doc.setTextColor(15, 40, 80);
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(11);
        doc.text(`${speaker.firstName} ${speaker.lastName}`, sx, y + 9);

        // Email
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(100, 110, 130);
        doc.setFontSize(9);
        doc.text(speaker.email ?? '', sx, y + 16);

        // Institution
        if (speaker.institution) {
          doc.setTextColor(80, 90, 110);
          doc.text(speaker.institution, sx, y + 22);
        }

        // Role badge
        if (speaker.role) {
          const roleX = pageWidth - margin - 4;
          doc.setFillColor(239, 246, 255);
          const roleWidth = doc.getStringUnitWidth(speaker.role) * 9 * 0.352 + 8;
          doc.roundedRect(roleX - roleWidth, y + 5, roleWidth, 7, 2, 2, 'F');
          doc.setTextColor(59, 130, 246);
          doc.setFontSize(8);
          doc.text(speaker.role, roleX - roleWidth / 2, y + 10, { align: 'center' });
        }

        // Subject
        if (speaker.subject) {
          doc.setFont('helvetica', 'italic');
          doc.setTextColor(130, 140, 160);
          doc.setFontSize(9);
          doc.text(speaker.subject, sx, y + 30, { maxWidth: contentWidth - 30 });
        }

        y += cardHeight + 5;

        // Separator between speakers (not after last)
        if (idx < speakers.length - 1) {
          doc.setDrawColor(230, 235, 245);
          doc.line(margin + 26, y, pageWidth - margin, y);
          y += 3;
        }
      });
    }

    // ── Footer on every page ───────────────────────────────
    const totalPages = (doc.internal as any).getNumberOfPages();
    for (let i = 1; i <= totalPages; i++) {
      doc.setPage(i);

      // Footer band
      doc.setFillColor(15, 40, 80);
      doc.rect(0, pageHeight - 14, pageWidth, 14, 'F');
      doc.setFillColor(59, 130, 246);
      doc.rect(0, pageHeight - 14, pageWidth, 2, 'F');

      doc.setTextColor(180, 200, 240);
      doc.setFont('helvetica', 'normal');
      doc.setFontSize(8);
      doc.text(event.title, margin, pageHeight - 5);
      doc.text(`Page ${i} / ${totalPages}`, pageWidth - margin, pageHeight - 5, { align: 'right' });
      doc.text(formatDay(event.startDate), pageWidth / 2, pageHeight - 5, { align: 'center' });
    }

    const slug = event.title.toLowerCase().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '');
    doc.save(`programme-${slug}.pdf`);
  };

  // ─── Loading / Error states ────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center text-text-secondary">
          Loading event...
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (isError || !event) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center">
          <h1 className="text-3xl font-bold text-navy dark:text-white mb-4">
            {t('common.error') || 'Event not found'}
          </h1>
          <Link to="/evenements" className="text-accent-blue hover:underline">
            Back to events
          </Link>
        </div>
        <PublicFooter />
      </div>
    );
  }

  // ─── Main render ───────────────────────────────────────────────────────────
  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      {/* Hero */}
      <div className="h-96 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 opacity-10">
          <svg className="w-full h-full">
            <defs>
              <pattern id="event-grid" width="50" height="50" patternUnits="userSpaceOnUse">
                <circle cx="25" cy="25" r="1" fill="white" />
              </pattern>
            </defs>
            <rect width="100%" height="100%" fill="url(#event-grid)" />
          </svg>
        </div>
        <div className="text-white text-center relative z-10 px-6">
          <div className="mb-4 flex flex-wrap items-center justify-center gap-2">
            <Badge
              variant="default"
              className={clsx(typeBadgeClass(event.type), 'shadow-sm')}
            >
              {event.type}
            </Badge>
            <Badge
              variant={statusConfig[event.status as keyof typeof statusConfig]?.variant ?? 'info'}
              className="!bg-white !text-accent-blue"
            >
              {statusConfig[event.status as keyof typeof statusConfig]?.label ?? event.status}
            </Badge>
          </div>
          <h1 className="text-4xl md:text-5xl font-bold mb-2">{event.title}</h1>
          <p className="text-white/80 text-lg">{formatDay(event.startDate)}</p>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

          {/* ── Main Content ── */}
          <div className="lg:col-span-2 space-y-12">

            {/* Breadcrumb */}
            <div className="text-sm text-text-secondary">
              <Link to="/evenements" className="hover:text-accent-blue">
                Événements
              </Link>{' '}
              → {event.title}
            </div>

            {/* Description */}
            <div>
              <h2 className="text-2xl font-bold text-navy dark:text-white mb-4">Description</h2>
              <div className="prose dark:prose-invert max-w-none text-text-secondary dark:text-text-secondary leading-relaxed">
                <p className="whitespace-pre-wrap">{event.description}</p>
              </div>
            </div>

            {/* Programme scientifique */}
            {event.program && (
              <div>
                <h3 className="text-xl font-bold text-navy dark:text-white mb-4">
                  Programme scientifique
                </h3>
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-accent-blue whitespace-pre-wrap text-text-secondary leading-relaxed">
                  {event.program}
                </div>
              </div>
            )}

            {/* Photo filenames */}
            {photoFileNames.length > 0 && (
              <div>
                <div className="flex items-center justify-between mb-4">
                  <h3 className="text-xl font-bold text-navy dark:text-white">
                    Photos de l'événement
                  </h3>
                  <span className="text-sm text-text-secondary">{photoFileNames.length} fichiers</span>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  {photoFileNames.map((photoName) => (
                    <div
                      key={photoName}
                      className="p-4 rounded-lg border border-surface-border bg-white dark:bg-card flex items-center gap-3"
                    >
                      <div className="w-10 h-10 rounded-lg bg-accent-blue/10 flex items-center justify-center text-accent-blue flex-shrink-0">
                        <ImageIcon size={18} />
                      </div>
                      <div className="min-w-0">
                        <div className="text-sm font-medium text-navy dark:text-white truncate">
                          {photoName}
                        </div>
                        <div className="text-xs text-text-secondary">Fichier photo public</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Speakers */}
            <div>
              <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Intervenants</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {speakers.map((speaker, idx) => (
                  <Card key={idx}>
                    <CardContent className="p-4">
                      <div className="flex items-start gap-4">
                        <div className="w-16 h-16 rounded-full bg-navy dark:bg-accent-blue text-white flex items-center justify-center text-xl font-bold flex-shrink-0">
                          {getSpeakerInitials(speaker.firstName, speaker.lastName)}
                        </div>
                        <div className="flex-1 min-w-0">
                          <h4 className="font-bold text-navy dark:text-white">
                            {speaker.firstName} {speaker.lastName}
                          </h4>
                          <div className="space-y-1 mt-2">
                            <div className="flex items-center gap-2 text-sm text-text-secondary">
                              <Mail size={14} className="flex-shrink-0 text-accent-blue" />
                              <span className="truncate">{speaker.email}</span>
                            </div>
                            <div className="flex items-center gap-2 text-sm text-text-secondary">
                              <Briefcase size={14} className="flex-shrink-0 text-teal" />
                              <span>{speaker.institution ?? '—'}</span>
                            </div>
                          </div>
                          {speaker.role && (
                            <Badge variant="info" className="mt-2 text-xs">
                              {speaker.role}
                            </Badge>
                          )}
                          {speaker.subject && (
                            <p className="text-sm text-text-secondary mt-2 italic">
                              {speaker.subject}
                            </p>
                          )}
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </div>
          </div>

          {/* ── Sidebar ── */}
          <div className="space-y-6">
            <Card>
              <CardContent className="p-6 space-y-6">

                {/* Date */}
                <div>
                  <div className="flex items-start gap-3">
                    <Calendar size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Date</div>
                      <div className="text-sm text-text-secondary mt-1">
                        {formatDay(event.startDate)}
                      </div>
                      <div className="text-sm text-text-secondary">
                        {formatDate(event.startDate)} - {formatDate(event.endDate)}
                      </div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                {/* Location */}
                <div>
                  <div className="flex items-start gap-3">
                    <MapPin size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Lieu</div>
                      <div className="text-sm text-text-secondary mt-1 whitespace-pre-wrap">
                        {event.location}
                      </div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                {/* Speakers count */}
                <div>
                  <div className="flex items-start gap-3">
                    <Users size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Intervenants</div>
                      <div className="text-sm text-text-secondary mt-1">
                        {speakers.length} conférencier{speakers.length > 1 ? 's' : ''}
                      </div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                {/* Download button */}
                <button
                  onClick={handleDownloadProgram}
                  className="flex items-center justify-center gap-2 w-full px-4 py-3 bg-accent-blue text-white rounded-lg hover:bg-accent-blue/90 transition-colors"
                >
                  <Download size={18} />
                  Télécharger le programme
                </button>

              </CardContent>
            </Card>
          </div>

        </div>
      </div>

      <PublicFooter />
    </div>
  );
}